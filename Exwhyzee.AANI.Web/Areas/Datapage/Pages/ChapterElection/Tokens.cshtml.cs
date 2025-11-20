using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class TokensModel : PageModel
    {
        private readonly AaniDbContext _db;

        public TokensModel(AaniDbContext db)
        {
            _db = db;
        }

        public long ChapterId { get; set; }
        public int? ElectionId { get; set; }
        public Chapter? Chapter { get; set; }

        // Accredited list to display
        public List<ChapterAccreditedVoter> Accredited { get; set; } = new();

        // Optional: plain tokens mapping (accreditedId -> plain token).
        // Only set/populate this immediately after generation/export and only for admin view.
        public Dictionary<long, string>? PlainTokens { get; set; }

        public async Task OnGetAsync(long chapterId, int? electionId = null)
        {
            ChapterId = chapterId;
            ElectionId = electionId;

            Chapter = await _db.Chapters.FindAsync(chapterId);

            var q = _db.ChapterAccreditedVoters
                .Where(a => a.ChapterId == chapterId);

            if (electionId.HasValue)
                q = q.Where(a => a.ChapterElectionId == electionId.Value);

            Accredited = await q
                .Include(a => a.Participant)
                .OrderBy(a => a.Participant.Title)
                .ToListAsync();

            // PlainTokens remains null by default.
            // If you want to show plain tokens after generation, set PlainTokens before returning (e.g. populate from a service result).
        }
    }
}