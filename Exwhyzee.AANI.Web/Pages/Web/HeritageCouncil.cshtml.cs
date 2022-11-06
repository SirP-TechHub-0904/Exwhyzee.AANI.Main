using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.Web
{
    public class HeritageCouncilModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public HeritageCouncilModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<HeritageCouncil> HeritageCouncil { get; set; }

        public async Task OnGetAsync()
        {
            HeritageCouncil = await _context.HeritageCouncils.ToListAsync();
        }
    }
}
