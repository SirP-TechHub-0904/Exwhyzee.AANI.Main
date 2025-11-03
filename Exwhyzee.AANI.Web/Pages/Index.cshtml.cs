using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Slider> Sliders { get; set; }
        public IList<Blog> Blogs { get; set; }
        public IList<ExecutivePosition> ExecutivePositions { get; set; }
        public IList<Executive> Executive { get; set; }

        public async Task OnGetAsync()
        {
            Sliders = _context.Sliders.OrderBy(d => d.SortOrder).Where(x=>x.Show).ToList();
            Blogs = _context.Blogs
                .Include(x=>x.BlogCategory)
                .Include(x=>x.Comments)
                .OrderByDescending(d => d.Date).Take(5).ToList();

        }
    }
}


//https://weblayoutpro.com/judges/