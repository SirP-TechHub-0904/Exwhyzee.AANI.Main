using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]
    public class MembersModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly TokenService _tokenService;
        private readonly ILogger<MembersModel> _logger;
        private readonly UserManager<Participant> _userManager;

        public MembersModel(AaniDbContext context, TokenService tokenService, ILogger<MembersModel> logger, UserManager<Participant> userManager)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }
        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }
        public Exwhyzee.AANI.Domain.Models.ChapterElection ChapterElection { get;set;}
        public Chapter Chapter { get; set; } = default!;

        public IList<ChapterAccreditedVoter> Accredited { get; set; } = new List<ChapterAccreditedVoter>();

        // participants you can add (simple list; in real app provide search/paging)
        public IList<Participant> Participants { get; set; } = new List<Participant>();

        public async Task<IActionResult> OnGetAsync(long chapterId, long electionId)
        {
            ChapterId = chapterId;
            ElectionId = electionId;
            Chapter = await _context.Chapters.FindAsync(chapterId);
            if (Chapter == null) return NotFound();

            ChapterElection = await _context.ChapterElections.FindAsync(electionId);

            Accredited = await _context.ChapterAccreditedVoters
                .Include(a => a.Participant)
                .Where(a => a.ChapterId == chapterId && a.ChapterElectionId == electionId)
                .OrderBy(a => a.ParticipantId)
                .ToListAsync();

            // load participants not yet accredited (simple)
            var accreditedParticipantIds = Accredited.Select(a => a.ParticipantId).Where(x => x != null).ToList();
            Participants = await _userManager.Users
                .Where(a => a.ChapterId == chapterId)
                .Where(p => !accreditedParticipantIds.Contains(p.Id))
                .OrderBy(p => p.LastLogin)
                .ThenBy(p => p.FirstName)
                .Take(2000)
                .ToListAsync();

            return Page();
        }

        // Add a participant to accreditation pool (no token issued)
        public async Task<IActionResult> OnPostAddAccreditedAsync(string participantId, long chapterId, long electionId)
        {
            if (string.IsNullOrWhiteSpace(participantId)) return BadRequest("participantId required");

            var exists = await _context.ChapterAccreditedVoters
                .AnyAsync(a => a.ParticipantId == participantId && a.ChapterId == chapterId && a.ChapterElectionId == electionId);

            if (exists)
            {
                TempData["Message"] = "Participant is already accredited for this chapter.";
                return RedirectToPage(new { chapterId, electionId });
            }

            var acc = new ChapterAccreditedVoter
            {
                ParticipantId = participantId,
                ChapterId = chapterId,
                ChapterElectionId = electionId,
                Date = DateTime.UtcNow,
                Voted = false
            };

            _context.ChapterAccreditedVoters.Add(acc);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Participant added to accreditation pool.";
            return RedirectToPage(new { chapterId, electionId });
        }

        // Remove an accredited participant only if they haven't voted
        public async Task<IActionResult> OnPostRemoveAccreditedAsync(long accreditedId, long chapterId, long electionId)
        {
            // Validate inputs
            if (accreditedId <= 0)
            {
                TempData["Error"] = "Invalid accredited id.";
                return RedirectToPage(new { chapterId });
            }

            var acc = await _context.ChapterAccreditedVoters
                .Include(a => a.Participant)
                .FirstOrDefaultAsync(a => a.Id == accreditedId && a.ChapterId == chapterId);

            if (acc == null)
            {
                TempData["Error"] = "Accredited record not found.";
                return RedirectToPage(new { chapterId, electionId });
            }

            if (acc.Voted)
            {
                TempData["Error"] = "Cannot remove: participant has already voted.";
                return RedirectToPage(new { chapterId, electionId });
            }

            try
            {
                _context.ChapterAccreditedVoters.Remove(acc);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Removed {acc.Participant?.Fullname ?? acc.ParticipantId} from accreditation pool.";
                _logger.LogInformation("Removed accredited participant {ParticipantId} (AccreditedId={AccreditedId}) from chapter {ChapterId} by admin {Admin}",
                    acc.ParticipantId, acc.Id, chapterId, User?.Identity?.Name ?? "unknown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove accredited id {AccreditedId} from chapter {ChapterId}", accreditedId, chapterId);
                TempData["Error"] = "Failed to remove accredited participant. Try again.";
            }

            return RedirectToPage(new { chapterId, electionId });
        }

        // Generate & optionally send token for an accredited voter
        public async Task<IActionResult> OnPostGenerateTokenAsync(long accreditedId, long electionId, bool sendNow = true)
        {
            try
            {
                var plain = await _tokenService.GenerateAndOptionallySendAsync(accreditedId, electionId, sendNow);
                // Show the token to admin once (plaintext)
                TempData["TokenMessage"] = $"Generated token: {plain} (show this to voter once)";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate token failed for accreditedId {Id}", accreditedId);
                TempData["Error"] = "Failed to generate token.";
            }

            return RedirectToPage(new { chapterId = ChapterId, electionId = ElectionId });
        }

        // Bulk generate for checked accredited items (ids passed via form)
        public async Task<IActionResult> OnPostBulkGenerateAsync([FromForm] long[] accreditedIds, long electionId, int expiryDays = 7)
        {
            if (accreditedIds == null || accreditedIds.Length == 0)
            {
                TempData["Error"] = "No accredited voters selected.";
                return RedirectToPage(new { chapterId = ChapterId });
            }

            var tokens = await _tokenService.GenerateBulkAsync(accreditedIds, electionId, expiryDays);
            // For now show count; in real app you should present exported CSV with plain tokens (admin-only)
            TempData["Message"] = $"Generated {tokens.Count} tokens. Present or export them to send to voters.";
            // Optionally save the tokens to a CSV or return a file. For security do not log tokens.
            return RedirectToPage(new { chapterId = ChapterId, electionId = ElectionId });
        }
    }
}