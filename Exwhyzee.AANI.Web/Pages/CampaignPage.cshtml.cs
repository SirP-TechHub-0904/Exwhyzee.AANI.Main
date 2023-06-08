using Exwhyzee.AANI.Domain.Models;
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
    }
}
