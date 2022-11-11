using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Exwhyzee.AANI.Web.Pages.Shared.ViewComponents
{
    public class DateAgoViewComponent : ViewComponent
    {

        private readonly RoleManager<IdentityRole> _roleManager;

        public DateAgoViewComponent(
            RoleManager<IdentityRole> roleManager
            )
        {
            
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(DateTime date)
        {
            string treturn = date.ToString();
            DateTime value = date;
            bool approximate = true;
            StringBuilder sb = new StringBuilder();

            string suffix = (value > DateTime.Now) ? " from now" : " ago";

            TimeSpan timeSpan = new TimeSpan(Math.Abs(DateTime.Now.Subtract(value).Ticks));

            if (timeSpan.Days > 0)
            {
                sb.AppendFormat("{0} {1}", timeSpan.Days,
                  (timeSpan.Days > 1) ? "days" : "day");
                if (approximate)
                    treturn =  sb.ToString() + suffix;
            }
            if (timeSpan.Hours > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Hours, (timeSpan.Hours > 1) ? "hours" : "hour");
                if (approximate) treturn =  sb.ToString() + suffix;
            }
            if (timeSpan.Minutes > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Minutes, (timeSpan.Minutes > 1) ? "minutes" : "minute");
                if (approximate) treturn =  sb.ToString() + suffix;
            }
            if (timeSpan.Seconds > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Seconds, (timeSpan.Seconds > 1) ? "seconds" : "second");
                if (approximate) treturn =  sb.ToString() + suffix;
            }
            if (sb.Length == 0) treturn =  "right now";

            sb.Append(suffix);
            treturn = sb.ToString();
            ViewBag.tt = treturn;
            return View();
        }
    }
}
