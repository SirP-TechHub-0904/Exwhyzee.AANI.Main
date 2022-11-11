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
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AANI.Web.Pages
{
    public class AaniEventsModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration Configuration;
        public AaniEventsModel(UserManager<Participant> userManager, Data.AaniDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            Configuration = configuration;
        }

        public PaginatedList<Event>? Events { get; set; }
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
            IQueryable<Event> eventlist = from s in _context.Events.Include(x => x.EventComments)
                                                      .OrderByDescending(x => x.Date)
                                          select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                eventlist = eventlist.Where(s => s.Title.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                     
                                       );
            }

            AllCount = eventlist.Count();

            var pageSize = 10;
            Events = await PaginatedList<Event>.CreateAsync(
                eventlist.AsNoTracking(), pageIndex ?? 1, pageSize);

            PageIndex = pageIndex ?? 1;

            return Page();
        }

    }
}
