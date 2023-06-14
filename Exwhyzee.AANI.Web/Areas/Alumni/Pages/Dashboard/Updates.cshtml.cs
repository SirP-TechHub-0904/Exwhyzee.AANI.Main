using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
    public class UpdatesModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration Configuration;
        public UpdatesModel(UserManager<Participant> userManager, Data.AaniDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            Configuration = configuration;
        }

        public PaginatedList<Blog>? Blog { get; set; }
        public int AllCount { get; set; }
        public SEC SEC { get; set; }
        public string CurrentFilter { get; set; }
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
        public IList<BlogCategory> BlogCategory { get; set; }
        public async Task<IActionResult> OnGetAsync(string currentFilter, string searchString, int? pageIndex, long cid = 0)
        {

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            if (CurrentFilter == null)
            {
                CurrentFilter = "Search";
            }
            IQueryable<Blog> bloglist = from s in _context.Blogs
                                        .Include(x => x.BlogCategory)
                                        .Include(x => x.Comments)
                                                      .OrderByDescending(x => x.Date)
                                        select s;
            if(cid > 0)
            {
                bloglist = bloglist.Where(x => x.BlogCategoryId == cid);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                bloglist = bloglist.Where(s => s.Title.Contains(searchString)
                                       || s.Content.Contains(searchString)
                                       || s.ShortDescription.Contains(searchString)

                                       );
            }

            AllCount = bloglist.Count();

            var pageSize = 9; TotalPage = AllCount / pageSize;
            Blog = await PaginatedList<Blog>.CreateAsync(
                bloglist.AsNoTracking(), pageIndex ?? 1, pageSize);

            PageIndex = pageIndex ?? 1;

            BlogCategory = await _context.BlogCategories.ToListAsync();
            return Page();
        }

    }
}
