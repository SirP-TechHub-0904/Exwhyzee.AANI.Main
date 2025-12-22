using System.Threading.Tasks;
using Exwhyzee.AANI.Web.Data;
 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account.BirthdayTemplates
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly AaniDbContext _context;

        public DetailsModel(AaniDbContext context)
        {
            _context = context;
        }

        public BirthdayTemplate Template { get; set; } = new BirthdayTemplate();

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var t = await _context.BirthdayTemplates.FindAsync(id);
            if (t == null) return NotFound();
            Template = t;
            return Page();
        }
    }
}