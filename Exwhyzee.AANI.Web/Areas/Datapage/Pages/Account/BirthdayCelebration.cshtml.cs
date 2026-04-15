using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI,Birthdays")]

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
            var allParticipants = _userManager.Users
        .Include(x => x.SEC)
        .Where(x => x.AliveStatus == AliveStatus.Alive)
        .Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI);
            DateTime querydate = DateTime.Today;

            if (searchdate == null && alldate == null && month == null)
            {
                Participants = allParticipants
           .Where(u => u.DOB.Day == querydate.Day && u.DOB.Month == querydate.Month)
           .OrderByDescending(x => x.DOB.Month)
           .ThenBy(x => x.DOB.Day);

                searchdate = DateTime.UtcNow.Date.ToString("ddd dd MMM, yyyy") + " (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }
            else if (alldate == "loadall")
            {
                Participants = allParticipants
    .OrderByDescending(x => x.DOB.Month)
    .ThenBy(x => x.DOB.Day);

                searchdate = "ALL MEMBERS AND THEIR BIRTHDAY (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }
            else if (!string.IsNullOrEmpty(month))
            {
                int monthNumber = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;

                Participants = allParticipants
                    .Where(u => u.DOB.Month == monthNumber)
                    .OrderByDescending(x => x.DOB.Month)
                    .ThenBy(x => x.DOB.Day);

                searchdate = month.ToUpper() + " (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }
            else
            {
                querydate = DateTime.Parse(searchdate).Date;

                Participants = allParticipants
                    .Where(u => u.DOB.Day == querydate.Day && u.DOB.Month == querydate.Month)
                    .OrderByDescending(x => x.DOB.Month)
                    .ThenBy(x => x.DOB.Day);

                searchdate = querydate.ToString("ddd dd MMM, yyyy") + " (" + Participants.Count() + " BIRTHDAY" + (Participants.Count() > 1 ? "S)" : ")");
            }

            TempData["date"] = searchdate;

            DateTime today = DateTime.UtcNow.Date;
            DateTime nextDay = today.AddDays(1);
            DateTime nextTwoDays = today.AddDays(2);
            DateTime nextThreeDays = today.AddDays(3);
            DateTime nextFourDays = today.AddDays(4);

            D1 = today.ToString("yyyy-MM-dd");
            D2 = nextDay.ToString("yyyy-MM-dd");
            D3 = nextTwoDays.ToString("yyyy-MM-dd");
            D4 = nextThreeDays.ToString("yyyy-MM-dd");
            D5 = nextFourDays.ToString("yyyy-MM-dd");

            V1 = allParticipants.Count(u => u.DOB.Day == today.Day && u.DOB.Month == today.Month)
                + " Birthday" + (allParticipants.Count(u => u.DOB.Day == today.Day && u.DOB.Month == today.Month) > 1 ? "s" : "") + " for Today";

            V2 = allParticipants.Count(u => u.DOB.Day == nextDay.Day && u.DOB.Month == nextDay.Month)
                + " Birthday" + (allParticipants.Count(u => u.DOB.Day == nextDay.Day && u.DOB.Month == nextDay.Month) > 1 ? "s" : "") + " for Tomorrow";

            V3 = allParticipants.Count(u => u.DOB.Day == nextTwoDays.Day && u.DOB.Month == nextTwoDays.Month)
                + " Birthday" + (allParticipants.Count(u => u.DOB.Day == nextTwoDays.Day && u.DOB.Month == nextTwoDays.Month) > 1 ? "s" : "") + " for " + nextTwoDays.ToString("yyyy-MM-dd");

            V4 = allParticipants.Count(u => u.DOB.Day == nextThreeDays.Day && u.DOB.Month == nextThreeDays.Month)
                + " Birthday" + (allParticipants.Count(u => u.DOB.Day == nextThreeDays.Day && u.DOB.Month == nextThreeDays.Month) > 1 ? "s" : "") + " for " + nextThreeDays.ToString("yyyy-MM-dd");

            V5 = allParticipants.Count(u => u.DOB.Day == nextFourDays.Day && u.DOB.Month == nextFourDays.Month)
                + " Birthday" + (allParticipants.Count(u => u.DOB.Day == nextFourDays.Day && u.DOB.Month == nextFourDays.Month) > 1 ? "s" : "") + " for " + nextFourDays.ToString("yyyy-MM-dd");

            return Page();
        }
    }

}
