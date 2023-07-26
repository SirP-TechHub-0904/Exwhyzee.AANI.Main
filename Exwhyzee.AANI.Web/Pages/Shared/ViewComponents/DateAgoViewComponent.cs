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
            string treturn = date.ToString("yyyy, M, d");
             
            DateRepresentationGenerator generator = new DateRepresentationGenerator();
            string dateRepresentation = generator.GetDateRepresentation(date);

            ViewBag.tt = dateRepresentation;
            return View();
        }
    }

    public class DateRepresentationGenerator
    {
        public string GetDateRepresentation(DateTime date)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan timeSinceDate = currentDate - date;

            if (timeSinceDate.TotalMinutes < 1)
            {
                return "Few minutes ago";
            }
            else if (timeSinceDate.TotalDays < 1)
            {
                return $"Few hours ago";
            }
            else if (timeSinceDate.TotalDays < 30)
            {
                int res = (int)timeSinceDate.TotalDays;
                int mees = res + 1;
                return $"{mees} days ago";
            }
            else
            {
                return date.ToString("dd MMM yyyy");
            }
        }
    }

}
