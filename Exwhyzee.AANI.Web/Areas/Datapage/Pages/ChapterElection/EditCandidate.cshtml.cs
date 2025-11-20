using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Authorize(Roles = "Admin,MNI")]
    public class EditCandidateModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly ILogger<EditCandidateModel> _logger;

        public EditCandidateModel(AaniDbContext context, ILogger<EditCandidateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public long CandidateId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }

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

            // readonly participant id for display
            public string CandidateParticipantId { get; set; } = string.Empty;
            public string CandidateName { get; set; } = string.Empty;
        }

        // positions for dropdown
        public List<ElectionPosition> Positions { get; set; } = new List<ElectionPosition>();

        public async Task<IActionResult> OnGetAsync(long candidateId, long electionId, long chapterId)
        {
            CandidateId = candidateId;
            ElectionId = electionId;
            ChapterId = chapterId;

            var cand = await _context.ElectionCandidates
                .Include(c => c.CandidateParticipant)
                .FirstOrDefaultAsync(c => c.Id == candidateId && c.ElectionId == electionId);

            if (cand == null) return NotFound();

            CandidateEdit = new CandidateEditInput
            {
                CandidateId = cand.Id,
                Manifesto = cand.Manifesto,
                BallotOrder = cand.BallotOrder,
                PositionId = cand.PositionId,
                CandidateParticipantId = cand.CandidateParticipantId,
                CandidateName = cand.CandidateParticipant?.Fullname ?? cand.CandidateParticipantId
            };

            Positions = await _context.ElectionPositions
                .Where(p => p.ElectionId == electionId)
                .OrderBy(p => p.BallotOrder)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input.";
                return Page();
            }

            var cand = await _context.ElectionCandidates.FirstOrDefaultAsync(c => c.Id == CandidateEdit.CandidateId && c.ElectionId == ElectionId);
            if (cand == null)
            {
                TempData["Error"] = "Candidate not found.";
                return RedirectToPage("/ChapterElection/Candidates", new { area = "Datapage", electionId = ElectionId, chapterId = ChapterId });
            }

            // validate position belongs to election (if provided)
            if (CandidateEdit.PositionId.HasValue && CandidateEdit.PositionId.Value > 0)
            {
                var posExists = await _context.ElectionPositions.AnyAsync(p => p.Id == CandidateEdit.PositionId.Value && p.ElectionId == ElectionId);
                if (!posExists)
                {
                    TempData["Error"] = "Selected position is invalid.";
                    return RedirectToPage("/ChapterElection/Candidates", new { area = "Datapage", electionId = ElectionId, chapterId = ChapterId });
                }
            }

            cand.Manifesto = CandidateEdit.Manifesto;
            cand.BallotOrder = CandidateEdit.BallotOrder;
            cand.PositionId = (CandidateEdit.PositionId.HasValue && CandidateEdit.PositionId.Value > 0) ? CandidateEdit.PositionId : null;

            _context.ElectionCandidates.Update(cand);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Candidate updated.";
            _logger.LogInformation("Candidate {CandidateId} updated for election {ElectionId}", cand.Id, ElectionId);

            return RedirectToPage("/ChapterElection/Candidates", new { area = "Datapage", electionId = ElectionId, chapterId = ChapterId });
        }
    }
}