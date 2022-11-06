using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.Web
{
    public class PatronsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public PatronsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Patron> Patron { get; set; }

        public async Task OnGetAsync()
        {
            Patron = await _context.Patrons
                .Include(p => p.Participant).ThenInclude(x=>x.SEC).ToListAsync();
        }
    }
}
