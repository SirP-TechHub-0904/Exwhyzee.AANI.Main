using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class BirthdayCelebrationModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public BirthdayCelebrationModel(UserManager<Participant> userManager, Data.AaniDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IQueryable<Participant>? Participants { get; set; }

        public string D1 { get; set; }
        public string V1 { get; set; }

        public string D2 { get; set; }
        public string V2 { get; set; }

        public string D3 { get; set; }
        public string V3 { get; set; }

        public string D4 { get; set; }
        public string V4 { get; set; }

        public string D5 { get; set; }
        public string V5 { get; set; }

        public async Task<IActionResult> OnGetAsync(string searchdate = null, string alldate = null, string month = null)
        {
            DateTime querydate = DateTime.Today;

            if (searchdate == null && alldate == null && month == null)
            {
                Participants = from s in _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI)
                    .OrderByDescending(x => x.DOB.Month).ThenBy(x => x.DOB.Day) 
                                 .Where(u => u.DOB.Day == DateTime.UtcNow.Day && u.DOB.Month == DateTime.UtcNow.Month)
                               select s;


                searchdate = DateTime.UtcNow.Date.ToString("ddd dd MMM, yyyy") + " (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }
            else if (alldate == "loadall")
            {
                Participants = from s in _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI)
                   .OrderByDescending(x => x.DOB.Month).ThenBy(x => x.DOB.Day)
                               select s;
                searchdate = "ALL MEMBERS AND THEIR BIRTHDAY (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }
            else if (month != null)
            {
                string cmonth = month;
                int day = 1;
                int year = 2023;

                DateTime datec = DateTime.Parse($"{cmonth} {day}, {year}");


                Participants = from s in _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI)
                   .OrderByDescending(x => x.DOB.Month).ThenBy(x => x.DOB.Day)
                   .Where(u => u.DOB.Month == datec.Month)
                               select s;
                searchdate = month.ToUpper()+ " (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }
            else
            {
                querydate = DateTime.Parse(searchdate).Date;

                Participants = from s in _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI)
                   .OrderByDescending(x => x.DOB.Month).ThenBy(x => x.DOB.Day) 
                                .Where(u => u.DOB.Day == querydate.Day && u.DOB.Month == querydate.Month)
                               select s;
                searchdate = querydate.Date.ToString("ddd dd MMM, yyyy") + " (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }

            TempData["date"] = searchdate;


            DateTime today = DateTime.UtcNow.Date;
            D1 = today.ToString("yyyy-MM-dd");
            DateTime nextDay = today.AddDays(1);
            D2 = nextDay.ToString("yyyy-MM-dd");
            DateTime nextTwoDays = today.AddDays(2);
            D3 = nextTwoDays.ToString("yyyy-MM-dd");
            DateTime nextThreeDays = today.AddDays(3);
            D4 = nextThreeDays.ToString("yyyy-MM-dd");
            DateTime nextFiveDays = today.AddDays(4);
            D5 = nextFiveDays.ToString("yyyy-MM-dd");

            int todayday = Participants.Count(u => u.DOB.Day == today.Day && u.DOB.Month == today.Month);
            V1 = todayday.ToString() + " Birthday" + (todayday > 1 ? "s" : "") + " for Today";
            int countNextDay = Participants.Count(u => u.DOB.Day == nextDay.Day && u.DOB.Month == nextDay.Month);
            V2 = countNextDay.ToString() + " Birthday" + (countNextDay > 1 ? "s" : "") + " for Tomorrow";
            int countNextTwoDays = Participants.Count(u => u.DOB.Date >= nextDay.Date && u.DOB.Date <= nextTwoDays.Date);
            V3 = countNextTwoDays.ToString() + " Birthday" + (countNextTwoDays > 1 ? "s" : "") + " for " + nextTwoDays.Date.ToString("yyyy-MM-dd");
            int countNextThreeDays = Participants.Count(u => u.DOB.Date >= nextDay.Date && u.DOB.Date <= nextThreeDays.Date);
            V4 = countNextThreeDays.ToString() + " Birthday" + (countNextThreeDays > 1 ? "s" : "") + " for " + nextThreeDays.Date.ToString("yyyy-MM-dd");
            int countNextFiveDays = Participants.Count(u => u.DOB.Date >= nextDay.Date && u.DOB.Date <= nextFiveDays.Date);
            V5 = countNextFiveDays.ToString() + " Birthday" + (countNextFiveDays > 1 ? "s" : "") + " for " + nextFiveDays.Date.ToString("yyyy-MM-dd");

            return Page();
        }
    }

}
