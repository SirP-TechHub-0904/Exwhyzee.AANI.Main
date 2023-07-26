using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages
{
    public class PaperModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration Configuration;
        public PaperModel(UserManager<Participant> userManager, Data.AaniDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            Configuration = configuration;
        }
        public PaperCategory PaperCategory { get; set; }

        public PaginatedList<Paper>? Papers { get; set; }
        public int AllCount { get; set; }
        public SEC SEC { get; set; }
        public string CurrentFilter { get; set; }
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }

        public async Task<IActionResult> OnGetAsync(string currentFilter, string searchString, int? pageIndex, long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PaperCategory = await _context.paperCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (PaperCategory == null)
            {
                return NotFound();
            }
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            
            IQueryable<Paper> paperList = from s in _context.Papers.Include(x => x.PaperCategory)
                                          .Include(x=>x.Participant).ThenInclude(c=>c.SEC)
                                                      .OrderByDescending(x => x.Year).Where(x=>x.PaperCategoryId == id)
                                                      select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                paperList = paperList.Where(s => s.Title.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.Participant.Surname.Contains(searchString)
                                       || s.Participant.FirstName.Contains(searchString)
                                       || s.Participant.OtherName.Contains(searchString)

                                       );
            }

            AllCount = paperList.Count();

            var pageSize = 30; TotalPage = AllCount / pageSize;
            Papers = await PaginatedList<Paper>.CreateAsync(
                paperList.AsNoTracking().OrderByDescending(x=>x.Year), pageIndex ?? 1, pageSize);

            PageIndex = pageIndex ?? 1;

            return Page();
        }

    }
}
