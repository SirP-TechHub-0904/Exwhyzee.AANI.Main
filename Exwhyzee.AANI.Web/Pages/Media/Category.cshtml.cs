using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.Media
{
    public class CategoryModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CategoryModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }
        public ContactSettingsModel ContactSettings { get; set; }

        public IList<BlogCategory> BlogCategory { get; set; }

        public async Task OnGetAsync()
        {
            BlogCategory = await _context.BlogCategories.ToListAsync();
            ContactSettings = await _context.ContactSettings
              .FirstOrDefaultAsync();
        }
    }
}
