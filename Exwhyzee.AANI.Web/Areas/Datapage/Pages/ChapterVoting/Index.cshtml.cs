using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterVoting
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _db;
        public IndexModel(AaniDbContext db) => _db = db;

        // optional: election id passed in route or query
        [BindProperty(SupportsGet = true)]
        public long? ElectionId { get; set; }

        // basic election info for the index page (when electionId provided)
        public Exwhyzee.AANI.Domain.Models.ChapterElection? Election { get; set; }

        // when no electionId provided and user is logged in, show elections user is accredited to
        public List<Exwhyzee.AANI.Domain.Models.ChapterElection> AccreditedElections { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(long? electionId)
        {
            ElectionId = electionId;

            if (ElectionId.HasValue)
            {
                Election = await _db.ChapterElections
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == ElectionId.Value);
                if (Election == null) return NotFound();
                return Page();
            }

            // No electionId provided — if user is authenticated, show elections they are accredited to
            if (User?.Identity?.IsAuthenticated == true)
            {
                // get current user id from claims (should match Participant.Id stored in ChapterAccreditedVoter.ParticipantId)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    // find distinct election ids the user is accredited for
                    var electionIds = await _db.ChapterAccreditedVoters
                        .AsNoTracking()
                        .Where(a => a.ParticipantId == userId && a.ChapterElectionId != null)
                         .Select(a => a.ChapterElectionId!.Value)
                        .Distinct()
                        .ToListAsync();

                    if (electionIds.Any())
                    {
                        AccreditedElections = await _db.ChapterElections
                            .AsNoTracking()
                            .Where(x=>x.ElectionStatus == Domain.Enums.ElectionStatus.Open)
                            .Where(e => electionIds.Contains(e.Id))
                            .OrderBy(e => e.StartAt)
                            .ToListAsync();
                    }
                }
            }

            // If not authenticated or no accredited elections, AccreditedElections will be empty and view will handle it
            return Page();
        }
    }
}