using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class PaperListModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration Configuration;
        public PaperListModel(UserManager<Participant> userManager, Data.AaniDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            Configuration = configuration;
        }
        public PaperCategory PaperCategory { get; set; }

        public PaginatedList<Paper>? Papers { get; set; }
        public IList<Paper> MyPapers { get; set; }
        public int AllCount { get; set; }
        public SEC SEC { get; set; }
        public string CurrentFilter { get; set; }
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }

        public async Task<IActionResult> OnGetAsync(string currentFilter, string searchString, int? pageIndex, long? id)
        {

             var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user");
            }
           

            if (id == null)
            {
                return NotFound();
            }

            PaperCategory = await _context.paperCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (PaperCategory == null)
            {
                return NotFound();
            }
             MyPapers = await _context.Papers.Where(x=>x.PaperCategoryId == id).Where(x=>x.ParticipantId == user.Id).ToListAsync();

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
                                                      .OrderByDescending(x => x.Year).Where(x=>x.PaperCategoryId == id).Where(x=>x.ParticipantId != user.Id)
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
