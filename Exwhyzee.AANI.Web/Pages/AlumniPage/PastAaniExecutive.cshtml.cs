using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.AlumniPage
{
    public class PastAaniExecutiveModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public PastAaniExecutiveModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<PastExecutiveYear> PastExecutiveYear { get; set; }

        public async Task OnGetAsync()
        {
            PastExecutiveYear = await _context.PastExecutiveYear
                .Include(e => e.PastExecutiveMembers).ThenInclude(x => x.Participant).ThenInclude(c=>c.SEC).ToListAsync();
        }
    }
}
