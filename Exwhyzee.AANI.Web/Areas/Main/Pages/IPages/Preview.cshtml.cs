using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.IPages
{
    public class PreviewModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public PreviewModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }
        public WebPage WebPage { get; set; }
        public PageSection PageSection { get; set; }
        public string Templatechoose { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id, long? secid)
        {
            
              if (id != null)
            {


                WebPage = await _context.WebPages
                    .Include(w => w.PageCategory)
                    .Include(w => w.PageSections)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (WebPage == null)
                {
                    return NotFound();
                }
            }
            if (secid != null)
            {


                PageSection = await _context.PageSections
                    .Include(w => w.PageSectionLists)
                    .Include(w => w.WebPage)
                    .FirstOrDefaultAsync(m => m.Id == secid);

                if (PageSection == null)
                {
                    return NotFound();
                }
            }
            return Page();
        }
    }
}
