using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.AlumniPage
{
    public class AaniExecutiveModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public AaniExecutiveModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Executive> Executive { get; set; }

        public async Task OnGetAsync()
        {
            Executive = await _context.Executives
                .Include(e => e.ExecutivePosition)
                .Include(e => e.Participant)
                .ThenInclude(x => x.SEC).ToListAsync();
        }
    }
}
