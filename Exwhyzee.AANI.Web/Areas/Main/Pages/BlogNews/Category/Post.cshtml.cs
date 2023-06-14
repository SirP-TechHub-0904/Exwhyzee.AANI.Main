using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.BlogNews.Category
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class PostModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public PostModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }
        public BlogCategory BlogCategory { get; set; }
        public IList<Blog> Blogs { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BlogCategory = await _context.BlogCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (BlogCategory == null)
            {
                return NotFound();
            }
            Blogs = await _context.Blogs.Where(x=>x.BlogCategoryId == id).OrderByDescending(d => d.Date).ToListAsync();
            return Page();
        }
    }
}
