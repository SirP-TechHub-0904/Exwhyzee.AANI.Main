using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AANI.Web.Pages.Media
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration Configuration;
        public DetailsModel(UserManager<Participant> userManager, Data.AaniDbContext context, IConfiguration configuration)
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
        public ContactSettingsModel ContactSettings { get; set; }

        public string CategoryName { get; set; }
        public async Task<IActionResult> OnGetAsync(string currentFilter, string searchString, int? pageIndex, long? catId)
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

            if(catId != null && catId > 0)
            {
                bloglist = bloglist.Where(x => x.BlogCategoryId == catId);

                //get the category details
                var category = await _context.BlogCategories.FirstOrDefaultAsync(x => x.Id == catId);
                if (category != null)
                {
                    CategoryName = category.Title;
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                bloglist = bloglist.Where(s => s.Title.Contains(searchString)
                                       || s.Content.Contains(searchString)
                                       || s.ShortDescription.Contains(searchString)

                                       );
            }

            AllCount = bloglist.Count();

            var pageSize = 10; TotalPage = AllCount / pageSize;
            Blog = await PaginatedList<Blog>.CreateAsync(
                bloglist.AsNoTracking(), pageIndex ?? 1, pageSize);

            PageIndex = pageIndex ?? 1;
            ContactSettings = await _context.ContactSettings
              .FirstOrDefaultAsync();
            return Page();
        }

    }
}
