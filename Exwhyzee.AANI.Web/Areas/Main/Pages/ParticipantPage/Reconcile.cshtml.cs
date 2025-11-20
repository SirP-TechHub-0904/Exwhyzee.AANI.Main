using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    //public class ReconcileModel : PageModel
    public class ReconcileModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public ReconcileModel(UserManager<Participant> userManager, AaniDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IQueryable<Participant>? Participants { get; set; }
         
        public async Task<IActionResult> OnGetAsync(AliveStatus aliveStatus = 0, GenderStatus genderStatus = 0, UserStatus userStatus = 0, VerificationStatus verificationStatus = 0, long chapterid = 0, long secid = 0)
        {
            Participants = _userManager.Users.Include(x => x.SEC).Where(x => x.Email != "jinmcever@gmail.com").AsQueryable();
            var DataParticipants = await _userManager.Users.AsNoTracking().Include(x => x.SEC).Where(x => x.Email.Contains("@aani") && x.EmergencyContactEmail != null).ToListAsync();
            int cu = DataParticipants.Count();
            foreach(var x in DataParticipants)
            {
                var user = await _userManager.FindByIdAsync(x.Id);
                user.Email = x.EmergencyContactEmail;
                await _userManager.UpdateAsync(user);
            }

            
            //await Task.Run();
            TempData["data"] = "ALL MEMBERS";

            return Page();
        }

    }

}
