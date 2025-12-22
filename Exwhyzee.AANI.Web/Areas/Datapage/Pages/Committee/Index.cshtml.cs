using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Committee
{
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;

        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        public IList<Exwhyzee.AANI.Domain.Models.Committee> Committees { get; set; } = new List<Exwhyzee.AANI.Domain.Models.Committee>();

        public async Task OnGetAsync()
        {
            Committees = await _context.Committees
                .Include(c => c.Members)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}