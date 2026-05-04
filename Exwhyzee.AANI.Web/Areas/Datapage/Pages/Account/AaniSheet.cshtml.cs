using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI,AANI")]
    public class AaniSheetModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public AaniSheetModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Participant> Participants { get; set; } = new();
        public List<SelectListItem> SecList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public long? SecId { get; set; }

        public string PageTitle { get; set; } = "AANI SHEET";

        public async Task<IActionResult> OnGetAsync()
        {
            SecList = await _context.SECs
                .AsNoTracking()
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.Number)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"SEC {x.Number} ({x.Year})"
                })
                .ToListAsync();

            var query = _userManager.Users
                .AsNoTracking()
                .Include(x => x.SEC)
                .Where(x => x.MniStatus == MniStatus.MNI)
                .AsQueryable();

            if (SecId.HasValue && SecId.Value > 0)
            {
                query = query.Where(x => x.SECId == SecId.Value);

                var sec = await _context.SECs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == SecId.Value);

                if (sec != null)
                {
                    PageTitle = $"AANI SHEET - SEC {sec.Number} ({sec.Year})";
                }
                Participants = await query
               .OrderBy(x => x.Surname)
               .ThenBy(x => x.FirstName)
               .ThenBy(x => x.OtherName)
               .ToListAsync();
            }
            else
            {
                PageTitle = "AANI SHEET - SELECT A SEC";
            }



            return Page();
        }
    }
}