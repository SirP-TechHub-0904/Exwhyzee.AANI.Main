using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterVoting
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class BallotModel : PageModel
    {
        private readonly AaniDbContext _db;
        public BallotModel(AaniDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }

        public Exwhyzee.AANI.Domain.Models.ChapterElection? Election { get; set; }

        // positions with candidates
        public List<ElectionPosition> Positions { get; set; } = new();

        // small error message
        public string? ErrorMessage { get; set; }

        // Display (GET)
        public async Task<IActionResult> OnGetAsync(long electionId)
        {
            ElectionId = electionId;

            var sessionId = Request.Cookies["VoteSessionId"];
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToPage("./EnterToken", new { electionId = ElectionId });
            }

            var now = DateTime.UtcNow;
            var session = await _db.VoteSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.ExpiresAt > now);

            if (session == null)
            {
                Response.Cookies.Delete("VoteSessionId");
                return RedirectToPage("./EnterToken", new { electionId = ElectionId });
            }

            if (session.ElectionId != ElectionId)
            {
                return RedirectToPage("./Index", new { electionId = session.ElectionId });
            }

            Election = await _db.ChapterElections
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == ElectionId);

            if (Election == null) return NotFound();

            Positions = await _db.Set<ElectionPosition>()
                .AsNoTracking()
                .Include(p => p.Candidates)
                .ThenInclude(c => c.CandidateParticipant)
                .Where(p => p.ElectionId == ElectionId && p.IsActive)
                .OrderBy(p => p.BallotOrder)
                .ToListAsync();
            // Prevent caching of this page so back/forward and proxies don't serve stale ballots
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return Page();
        }

        // Submit (POST)
        public async Task<IActionResult> OnPostAsync(long electionId)
        {
            ElectionId = electionId;

            var sessionId = Request.Cookies["VoteSessionId"];
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToPage("./EnterToken", new { electionId = ElectionId });
            }

            var now = DateTime.UtcNow;
            var session = await _db.VoteSessions.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null || session.ExpiresAt <= now)
            {
                Response.Cookies.Delete("VoteSessionId");
                return RedirectToPage("./EnterToken", new { electionId = ElectionId });
            }

            // Reload positions for validation
            Positions = await _db.Set<ElectionPosition>()
                .AsNoTracking()
                .Include(p => p.Candidates)
                .Where(p => p.ElectionId == ElectionId && p.IsActive)
                .OrderBy(p => p.BallotOrder)
                .ToListAsync();

            // Collect selections: each position input is named "pos_{positionId}"
            var selections = new List<long>();
            foreach (var pos in Positions)
            {
                var key = $"pos_{pos.Id}";
                var value = Request.Form[key].FirstOrDefault();
                if (!string.IsNullOrEmpty(value) && long.TryParse(value, out var cid))
                {
                    selections.Add(cid);
                }
            }

            if (!selections.Any())
            {
                ErrorMessage = "Please make at least one selection.";
                return Page();
            }

            // Validate that selected candidate ids belong to this election
            var validCount = await _db.Set<ElectionCandidate>()
                .AsNoTracking()
                .CountAsync(c => selections.Contains(c.Id) && c.ElectionId == session.ElectionId);

            if (validCount != selections.Count)
            {
                ErrorMessage = "Invalid selection detected.";
                return Page();
            }

            // TODO: optionally validate per-position constraints (MaxChoices) if you add that field later.

            // Atomic claim + insert vote (same approach as before)
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var affected = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE ChapterAccreditedVoters
                    SET Voted = 1, DateVoted = {now}
                    WHERE Id = {session.AccreditedVoterId}
                      AND Voted = 0
                      AND TokenExpiresAt > {now}
                      AND VoteTokenHash = {session.TokenHash}
                ");

                if (affected != 1)
                {
                    await tx.RollbackAsync();
                    Response.Cookies.Delete("VoteSessionId");
                    return RedirectToPage("./EnterToken", new { electionId = ElectionId });
                }

                var vote = new Vote
                {
                    ElectionId = session.ElectionId,
                    ChapterAccreditedVoterId = session.AccreditedVoterId,
                    CastAt = now
                };
                _db.Votes.Add(vote);
                await _db.SaveChangesAsync();

                // Map chosen candidate ids to VoteChoice entries
                int order = 1;
                foreach (var candidateId in selections)
                {
                    _db.VoteChoices.Add(new VoteChoice
                    {
                        VoteId = vote.Id,
                        CandidateId = candidateId,
                        Position = order++
                    });
                }
                await _db.SaveChangesAsync();

                // delete session
                _db.VoteSessions.Remove(session);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                Response.Cookies.Delete("VoteSessionId");
                return RedirectToPage("./Receipt", new { voteId = vote.Id });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}