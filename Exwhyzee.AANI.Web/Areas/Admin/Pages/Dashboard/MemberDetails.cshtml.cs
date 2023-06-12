using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AANI.Web.Areas.Admin.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class MemberDetailsModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;

        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public MemberDetailsModel(
            UserManager<Participant> userManager,
            AaniDbContext context)
        {
            _userManager = userManager;

            _context = context;
        }


        public Participant Participant { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                var user = await _userManager.GetUserAsync(User);
                id = user.Id;
                TempData["profile"] = "MY PROFILE";
            }
            TempData["profile"] = "MEMBER PROFILE";
            Participant = await _userManager.FindByIdAsync(id);

            if (Participant == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
