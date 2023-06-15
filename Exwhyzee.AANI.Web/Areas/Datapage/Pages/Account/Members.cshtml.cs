using Amazon.S3;
using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class MembersModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public MembersModel(UserManager<Participant> userManager, Data.AaniDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IQueryable<Participant>? Participants { get; set; }
        public int AllAlumni { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Alive { get; set; }
        public int Dead { get; set; }
        public int Active { get; set; }
        public int GenderQuery { get; set; }
        public async Task<IActionResult> OnGetAsync(AliveStatus aliveStatus = 0, GenderStatus genderStatus = 0, ActiveStatus activeStatus = 0, long chapterid = 0, long secid = 0)
        {
            Participants = _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).OrderBy(x=>x.Surname).AsQueryable();
            var DataParticipants = _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).AsQueryable();
            

            if (activeStatus == ActiveStatus.Active)
            {
                Participants = Participants.Where(x => x.ActiveStatus == ActiveStatus.Active).AsQueryable();

                TempData["data"] = "ACTIVE MEMBERS";
                 
            }else  if (aliveStatus== AliveStatus.Alive)
            {
                Participants = Participants.Where(x => x.AliveStatus == AliveStatus.Alive).AsQueryable();

                TempData["data"] = "MEMBERS ALIVE";
                 
            }else if (aliveStatus == AliveStatus.Dead)
            {
                Participants = Participants.Where(x => x.AliveStatus == AliveStatus.Dead).AsQueryable();

                TempData["data"] = "MEMBERS DEAD";
                
            }else if (genderStatus == GenderStatus.Male)
            {
                Participants = Participants.Where(x => x.GenderStatus == GenderStatus.Male).AsQueryable();

                TempData["data"] = "MALE MEMBERS";
                GenderQuery = 1;
            }
            else if (genderStatus == GenderStatus.Female)
            {
                Participants = Participants.Where(x => x.GenderStatus == GenderStatus.Female).AsQueryable();

                TempData["data"] = "FEMALE MEMBERS";
                GenderQuery = 2;
            }
            else if (chapterid > 0)
            {
                Participants = Participants.Where(x => x.ChapterId == chapterid).AsQueryable();
                var chapter = await _context.Chapters.FirstOrDefaultAsync(x => x.Id == chapterid);
                if(chapter == null)
                {
                    return RedirectToPage("./Members");
                }
                TempData["data"] = "MEMBERS OF " + chapter.State.ToUpper() +" CHAPTER";

                
            }else if (secid > 0)
            {
                Participants = Participants.Where(x => x.SECId == secid).AsQueryable();
                var secs = await _context.SECs.FirstOrDefaultAsync(x => x.Id == secid);
                if (secs == null)
                {
                    return RedirectToPage("./Members");
                }
                TempData["data"] = secs.Number.ToUpper() + " ("+secs.Year+") "+ Participants.Count() + " MEMBERS";

                return Page();
            }
            else
            {
                TempData["data"] = "ALL MEMBERS";
            }
            //await Task.Run();
            
            AllAlumni = await Participants.CountAsync();
            Active = await Participants.Where(x => x.ActiveStatus == Domain.Enums.ActiveStatus.Active).CountAsync();
            Alive = await Participants.Where(x => x.AliveStatus == Domain.Enums.AliveStatus.Alive).CountAsync();
            Dead = await Participants.Where(x => x.AliveStatus == Domain.Enums.AliveStatus.Dead).CountAsync();
            Male = await Participants.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Male).CountAsync();
            Female = await Participants.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Female).CountAsync();

            return Page();
        }
    }
}
