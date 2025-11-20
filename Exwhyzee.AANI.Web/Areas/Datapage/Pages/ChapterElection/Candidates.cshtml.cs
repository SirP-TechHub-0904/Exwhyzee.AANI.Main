using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Authorize(Roles = "Admin,MNI")]
    public class CandidatesModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;
        private readonly ILogger<CandidatesModel> _logger;

        public CandidatesModel(AaniDbContext context, UserManager<Participant> userManager, ILogger<CandidatesModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }

        public Exwhyzee.AANI.Domain.Models.ChapterElection? Election { get; set; }

        public List<ElectionPosition> Positions { get; set; } = new List<ElectionPosition>();

        [BindProperty]
        public CandidateEditInput CandidateEdit { get; set; } = new CandidateEditInput();

        public class CandidateEditInput
        {
            public long CandidateId { get; set; }

            [StringLength(4000)]
            public string? Manifesto { get; set; }

            public int BallotOrder { get; set; } = 0;

            // allow changing the position (null = unassigned)
            public long? PositionId { get; set; }
        }

        // EDIT bind models
        [BindProperty]
        public PositionEditInput PositionEdit { get; set; } = new PositionEditInput();

        public class PositionEditInput
        {
            public long PositionId { get; set; }

            [Required]
            [StringLength(250)]
            public string Name { get; set; } = string.Empty;

            [StringLength(150)]
            public string? Slug { get; set; }

            [StringLength(2000)]
            public string? Description { get; set; }

            public int Seats { get; set; } = 1;
            public int MaxChoices { get; set; } = 1;
            public int BallotOrder { get; set; } = 0;
            public bool IsActive { get; set; } = true;
        }
        // participants usable for candidate selection (not yet candidates in this election)
        public List<Participant> AvailableParticipants { get; set; } = new List<Participant>();

        // inputs
        [BindProperty]
        public PositionInput NewPosition { get; set; } = new PositionInput();

        public class PositionInput
        {
            [Required]
            [StringLength(250)]
            public string Name { get; set; } = string.Empty;

            [StringLength(150)]
            public string? Slug { get; set; }

            [StringLength(2000)]
            public string? Description { get; set; }

            public int Seats { get; set; } = 1;
            public int MaxChoices { get; set; } = 1;
            public int BallotOrder { get; set; } = 0;
            public bool IsActive { get; set; } = true;
        }

        [BindProperty]
        public CandidateInput NewCandidate { get; set; } = new CandidateInput();

        public class CandidateInput
        {
            [Required]
            public string CandidateParticipantId { get; set; } = string.Empty;

            // optional position id (0 = unassigned)
            public long? PositionId { get; set; }

            [StringLength(4000)]
            public string? Manifesto { get; set; }

            public int BallotOrder { get; set; } = 0;
        }

        public async Task<IActionResult> OnGetAsync(long electionId, long chapterId)
        {
            ElectionId = electionId;
            ChapterId = chapterId;

            // load and validate election
            Election = await _context.ChapterElections
                .Include(e => e.Candidates)
                    .ThenInclude(c => c.CandidateParticipant)
                .Include(e => e.Candidates)
                    .ThenInclude(c => c.Position)
                .FirstOrDefaultAsync(e => e.Id == electionId && e.ChapterId == chapterId);

            if (Election == null) return NotFound();

            // load positions with candidates
            Positions = await _context.ElectionPositions
                .Where(p => p.ElectionId == electionId)
                .Include(p => p.Candidates)
                    .ThenInclude(c => c.CandidateParticipant)
                .OrderBy(p => p.BallotOrder)
                .ToListAsync();

            // available participants for adding as candidates: exclude those already candidates for this election
            var existingCandidateParticipantIds = await _context.ElectionCandidates
                .Where(c => c.ElectionId == electionId)
                .Select(c => c.CandidateParticipantId)
                .ToListAsync();

            AvailableParticipants = await _userManager.Users
                .Where(c => c.ChapterId == chapterId)
                .Where(u => !existingCandidateParticipantIds.Contains(u.Id))
                .OrderBy(u => u.LastLogin)
                .ThenBy(u => u.FirstName)
                .Take(2000)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddPositionAsync(long electionId, long chapterId)
        {
            ElectionId = electionId;
            ChapterId = chapterId;

            Election = await _context.ChapterElections.FindAsync(electionId);
            if (Election == null || Election.ChapterId != chapterId) return NotFound();

             
            var pos = new ElectionPosition
            {
                ElectionId = ElectionId,
                Name = NewPosition.Name, 
                Description = NewPosition.Description, 
                BallotOrder = NewPosition.BallotOrder,
                IsActive = NewPosition.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.ElectionPositions.Add(pos);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Position '{pos.Name}' added.";
            _logger.LogInformation("Admin added position {Position} to election {ElectionId}", pos.Name, ElectionId);

            return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
        }

        public async Task<IActionResult> OnPostRemovePositionAsync(long positionId, long electionId, long chapterId)
        {
            ElectionId = electionId;
            ChapterId = chapterId;

            var pos = await _context.ElectionPositions
                .Include(p => p.Candidates)
                .FirstOrDefaultAsync(p => p.Id == positionId && p.ElectionId == electionId);

            if (pos == null)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            if (pos.Candidates != null && pos.Candidates.Any())
            {
                TempData["Error"] = $"Cannot remove position '{pos.Name}' because it has {pos.Candidates.Count} candidate(s). Remove or reassign candidates first.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            _context.ElectionPositions.Remove(pos);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Position '{pos.Name}' removed.";
            _logger.LogInformation("Admin removed position {PositionId} from election {ElectionId}", positionId, ElectionId);

            return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
        }

        public async Task<IActionResult> OnPostAddCandidateAsync(long electionId, long chapterId)
        {
            ElectionId = electionId;
            ChapterId = chapterId;

            Election = await _context.ChapterElections.FindAsync(electionId);
            if (Election == null || Election.ChapterId != chapterId) return NotFound();

            

            // duplicate candidate check
            var exists = await _context.ElectionCandidates.AnyAsync(c => c.ElectionId == electionId && c.CandidateParticipantId == NewCandidate.CandidateParticipantId);
            if (exists)
            {
                TempData["Error"] = "Selected participant is already a candidate in this election.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            // validate optional position belongs to same election
            if (NewCandidate.PositionId.HasValue && NewCandidate.PositionId.Value > 0)
            {
                var posExists = await _context.ElectionPositions.AnyAsync(p => p.Id == NewCandidate.PositionId.Value && p.ElectionId == electionId);
                if (!posExists)
                {
                    TempData["Error"] = "Selected position is invalid.";
                    return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
                }
            }

            var candidate = new ElectionCandidate
            {
                ElectionId = electionId,
                CandidateParticipantId = NewCandidate.CandidateParticipantId,
                PositionId = (NewCandidate.PositionId.HasValue && NewCandidate.PositionId.Value > 0) ? NewCandidate.PositionId : null,
                Manifesto = NewCandidate.Manifesto,
                BallotOrder = NewCandidate.BallotOrder
            };

            _context.ElectionCandidates.Add(candidate);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Candidate added.";
            _logger.LogInformation("Admin added candidate {ParticipantId} to election {ElectionId}", candidate.CandidateParticipantId, electionId);

            return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
        }

        public async Task<IActionResult> OnPostRemoveCandidateAsync(long candidateId, long electionId, long chapterId)
        {
            ElectionId = electionId;
            ChapterId = chapterId;

            var cand = await _context.ElectionCandidates.FirstOrDefaultAsync(c => c.Id == candidateId && c.ElectionId == electionId);
            if (cand == null)
            {
                TempData["Error"] = "Candidate not found.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            _context.ElectionCandidates.Remove(cand);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Candidate removed.";
            _logger.LogInformation("Admin removed candidate {CandidateId} from election {ElectionId}", candidateId, electionId);

            return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
        }


        public async Task<IActionResult> OnPostEditPositionAsync(long electionId, long chapterId)
        {
            ElectionId = electionId;
            ChapterId = chapterId;

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid position edit data.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            var pos = await _context.ElectionPositions.FirstOrDefaultAsync(p => p.Id == PositionEdit.PositionId && p.ElectionId == electionId);
            if (pos == null)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            pos.Name = PositionEdit.Name;
            pos.Slug = PositionEdit.Slug;
            pos.Description = PositionEdit.Description; 
            pos.BallotOrder = PositionEdit.BallotOrder;
            pos.IsActive = PositionEdit.IsActive;
            pos.UpdatedAt = DateTime.UtcNow;

            _context.ElectionPositions.Update(pos);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Position '{pos.Name}' updated.";
            _logger.LogInformation("Admin edited position {PositionId} on election {ElectionId}", pos.Id, electionId);

            return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
        }

        public async Task<IActionResult> OnPostEditCandidateAsync(long electionId, long chapterId)
        {
            ElectionId = electionId;
            ChapterId = chapterId;

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid candidate edit data.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            var cand = await _context.ElectionCandidates.FirstOrDefaultAsync(c => c.Id == CandidateEdit.CandidateId && c.ElectionId == electionId);
            if (cand == null)
            {
                TempData["Error"] = "Candidate not found.";
                return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
            }

            // validate optional new position belongs to same election
            if (CandidateEdit.PositionId.HasValue && CandidateEdit.PositionId.Value > 0)
            {
                var posExists = await _context.ElectionPositions.AnyAsync(p => p.Id == CandidateEdit.PositionId.Value && p.ElectionId == electionId);
                if (!posExists)
                {
                    TempData["Error"] = "Selected position is invalid.";
                    return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
                }
            }

            cand.Manifesto = CandidateEdit.Manifesto;
            cand.BallotOrder = CandidateEdit.BallotOrder;
            cand.PositionId = (CandidateEdit.PositionId.HasValue && CandidateEdit.PositionId.Value > 0) ? CandidateEdit.PositionId : null;

            _context.ElectionCandidates.Update(cand);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Candidate updated.";
            _logger.LogInformation("Admin edited candidate {CandidateId} on election {ElectionId}", cand.Id, electionId);

            return RedirectToPage(new { electionId = ElectionId, chapterId = ChapterId });
        }




    }
}