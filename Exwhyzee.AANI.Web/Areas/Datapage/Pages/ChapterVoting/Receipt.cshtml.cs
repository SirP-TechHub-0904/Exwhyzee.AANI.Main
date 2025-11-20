using System;
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

    public class ReceiptModel : PageModel
    {
        private readonly AaniDbContext _db;

        public ReceiptModel(AaniDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public long VoteId { get; set; }

        public Vote? Vote { get; set; }

        // Friendly values for the view
        public string? ElectionTitle { get; set; }
        public string? ReceiptToken { get; set; }
        public DateTime? CastAt { get; set; }

        public async Task<IActionResult> OnGetAsync(long voteId)
        {
            VoteId = voteId;

            // Load vote with choices and candidate + participant and election details
            Vote = await _db.Votes
                .AsNoTracking()
                .Include(v => v.Election)
                .Include(v => v.Choices)
                    .ThenInclude(vc => vc.Candidate)
                        .ThenInclude(c => c.CandidateParticipant)
                .FirstOrDefaultAsync(v => v.Id == VoteId);

            if (Vote == null)
            {
                return NotFound();
            }

            ElectionTitle = Vote.Election?.Title;
            ReceiptToken = Vote.ReceiptToken;
            CastAt = Vote.CastAt.ToLocalTime();

            // Optionally clear any VoteSession cookie if present so this page can be opened after redirect
            if (Request.Cookies.ContainsKey("VoteSessionId"))
            {
                Response.Cookies.Delete("VoteSessionId");
            }

            return Page();
        }
    }
}