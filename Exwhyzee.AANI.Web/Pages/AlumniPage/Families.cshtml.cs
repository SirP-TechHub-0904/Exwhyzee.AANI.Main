using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.AlumniPage
{
    public class FamiliesModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public FamiliesModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;

        }
        public IList<CategoryFamiliesOnSEC> CategoryFamiliesOnSEC { get; set; }

        public async Task OnGetAsync()
        {
            CategoryFamiliesOnSEC = await _context.CategoryFamiliesOnSECs
                .Include(x=>x.SubCategories)
                .ThenInclude(c=>c.ParticipantFamiliesOnSECs)
                .ThenInclude(c=>c.Participant)
                .ThenInclude(c=>c.SEC)
                .ToListAsync();
        }

    }
}
