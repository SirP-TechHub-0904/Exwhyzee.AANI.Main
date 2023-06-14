using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class ContestantsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public ContestantsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Campain> Campain { get; set; }
        public ExecutivePosition ExecutivePosition { get; set; }

        public async Task OnGetAsync(long id)
        {
            Campain = await _context.Campains
                .Include(c => c.ExecutivePosition)
                .Include(c => c.Participant).ThenInclude(x => x.SEC).Where(x => x.ExecutivePositionId == id).ToListAsync();

            ExecutivePosition = await _context.ExecutivePositions.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<JsonResult> OnGetDetailsPicture(long id)
        {
            var user = _context.Campains.Include(x => x.Participant).Include(x => x.ExecutivePosition).FirstOrDefault(x => x.Id == id);

            var result = "<img class=\"img-fluid\" src=\"https://aani.ng//img/campaign.jpg\">";
            if (user.ImageUrl != null)
            {
                result = "<img class=\"img-fluid\" src=\"" + user.ImageUrl + "\">";
            }

            var outcome = new ResultDto
            {
                Title = "CAMPAIGN PHOTO OF " + user.Participant.Fullname +" ON " + user.ExecutivePosition.Position,
                Description = result
            };
            return new JsonResult(outcome);
        }
        public async Task<JsonResult> OnGetDetails(long id)
        {
            var user = _context.Campains.Include(x=>x.Participant).Include(x => x.ExecutivePosition).FirstOrDefault(x => x.Id == id);

            var result = "";
            if (user.Manifesto != null)
            {
                result = user.Manifesto;
            }

            var outcome = new ResultDto
            {
                Title = "MANIFESTO FOR " + user.Participant.Fullname + " ON " + user.ExecutivePosition.Position,
                Description = result
            };
            return new JsonResult(outcome);
        }
    }

    public class ResultDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

}
