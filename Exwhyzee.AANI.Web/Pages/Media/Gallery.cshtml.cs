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
    public class GalleryModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration Configuration;
        public GalleryModel(UserManager<Participant> userManager, Data.AaniDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            Configuration = configuration;
        }

        public PaginatedList<Gallery>? Gallery { get; set; }
        public int AllCount { get; set; }
        public SEC SEC { get; set; }
        public string CurrentFilter { get; set; }
        public int PageIndex { get; set; }
        public async Task<IActionResult> OnGetAsync(string currentFilter, string searchString, int? pageIndex)
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
            IQueryable<Gallery> galleryList = from s in _context.Galleries
                                                      .OrderByDescending(x => x.Date)
                                          select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                galleryList = galleryList.Where(s => s.Title.Contains(searchString)
                                      
                                       );
            }

            AllCount = galleryList.Count();

            var pageSize = 20;
            Gallery = await PaginatedList<Gallery>.CreateAsync(
                galleryList.AsNoTracking(), pageIndex ?? 1, pageSize);

            PageIndex = pageIndex ?? 1;

            return Page();
        }

    }
}
