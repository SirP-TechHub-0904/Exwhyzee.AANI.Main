using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI,AANI")]
    public class MemebersIDCardsModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly AaniDbContext _context;

        public MemebersIDCardsModel(UserManager<Participant> userManager, AaniDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<Participant> Participants { get; set; } = new();

        public int TotalIdCardGiven { get; set; }
        public int TotalPendingIdCard { get; set; }
        public int TotalMembers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Participants = await _userManager.Users
                               .Where(x => x.MniStatus == Exwhyzee.AANI.Domain.Enums.MniStatus.MNI)
                .OrderBy(x => x.IdCardDownloadedAt)
                .ThenBy(x => x.Surname)
                .ToListAsync();

            TotalMembers = Participants.Count;
            TotalIdCardGiven = Participants.Count(x => x.IDCardDownloaded);
            TotalPendingIdCard = Participants.Count(x => !x.IDCardDownloaded);

            return Page();
        }
    }
}