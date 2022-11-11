using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.AlumniPage
{
    public class SecAlumniModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration Configuration;
        public SecAlumniModel(UserManager<Participant> userManager, Data.AaniDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            Configuration = configuration;
        }

        public PaginatedList<Participant>? Participants { get; set; }
        public int AllCount { get; set; }
        public SEC SEC { get; set; }
        public string CurrentFilter { get; set; }
        public int PageIndex { get; set; }
        public async Task OnGetAsync(string currentFilter, string searchString, int? pageIndex)
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
            if(CurrentFilter == null)
            {
                CurrentFilter = "Search Alumni";
            }
            IQueryable<Participant> participantlist = from s in _userManager.Users.Include(x => x.SEC)
                                                      .OrderByDescending(x => x.SEC.Year).ThenBy(x => Convert.ToInt32(x.SEC.Number))
                                                      select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                participantlist = participantlist.Where(s => s.Surname.Contains(searchString)
                                       || s.Title.Contains(searchString)
                                       || s.FirstName.Contains(searchString)
                                       || s.OtherName.Contains(searchString)
                                       || s.Sponsor.Contains(searchString)

                                       );
            }

            AllCount = participantlist.Count();

            var pageSize = 40;
            Participants = await PaginatedList<Participant>.CreateAsync(
                participantlist.AsNoTracking(), pageIndex ?? 1, pageSize);

            PageIndex = pageIndex ?? 1;
        }
    }
}
