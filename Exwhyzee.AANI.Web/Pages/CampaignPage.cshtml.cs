using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages
{
    public class CampaignModel : PageModel
    {

        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CampaignModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

         public IList<ExecutivePosition> ExecutivePositions { get; set; }

        public async Task OnGetAsync()
        {
             ExecutivePositions = _context.ExecutivePositions.Include(x => x.Campains).ToList();

        }

        public async Task<JsonResult> OnGetDetails(long id)
        {
            var user = _context.Campains.Include(x => x.Participant).Include(x => x.ExecutivePosition).FirstOrDefault(x => x.Id == id);

            var result = "";
            if (user.Manifesto != null)
            {
                result = user.Manifesto;
            }
            var xresult = "<img class=\"img-fluid\" src=\"https://aani.ng//img/campaign.jpg\">";
            if (user.ImageUrl != null)
            {
                xresult = "<img class=\"img-fluid\" src=\"" + user.ImageUrl + "\">";
            }
            var outcome = new WebResultDto
            {
                Title = "MANIFESTO FOR " + user.Participant.Fullname + " ON " + user.ExecutivePosition.Position,
                Description = result,
                Image = xresult
            };
            return new JsonResult(outcome);
        }
    }

    public class WebResultDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
