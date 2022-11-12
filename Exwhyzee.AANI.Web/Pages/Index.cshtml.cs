using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity;

namespace Exwhyzee.AANI.Web.Pages
{
    public class IndexModel : PageModel
    {
       

        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Blog> Blogs { get; set; }

        public async Task OnGetAsync()
        {
            Blogs = _context.Blogs.OrderByDescending(d => d.Date).Take(5).ToList();
        }
    }
}