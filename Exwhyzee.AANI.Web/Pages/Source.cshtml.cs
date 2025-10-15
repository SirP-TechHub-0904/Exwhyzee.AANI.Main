using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages
{
    public class SourceModel : PageModel
    {
        private readonly AaniDbContext _context;

        public SourceModel(AaniDbContext context)
        {
            _context = context;
        }

        public WebPage WebPage { get; set; } 
        public ContactSettingsModel ContactSettings { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }
            if (id == 000)
            {
                return RedirectToPage("/Contact");
            }
            WebPage = await _context.WebPages.FirstOrDefaultAsync(m => m.Id == id);

            if (WebPage == null)
            {
                return NotFound();
            }
            ContactSettings = await _context.ContactSettings
                .FirstOrDefaultAsync();
            return Page();
        }
    }

}
