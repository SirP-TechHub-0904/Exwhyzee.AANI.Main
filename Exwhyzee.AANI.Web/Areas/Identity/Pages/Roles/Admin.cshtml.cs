using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AANI.Web.Areas.Identity.Pages.Roles
{
    public class AdminModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly ILogger<ReadonlyModel> _logger;
         private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public AdminModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {

            var user = new Participant
            {
                UserName = "nasiruladan@aani.ng",
                Email = "nasiruladan@aani.ng",
                PhoneNumber = "",
                FirstName = "NASIRU",
                Surname = "LADAN",
                OtherName = "",
                Title = "",
                Sponsor = "Admin",
                SECId = 18
            };
            Guid pass = Guid.NewGuid();
            var result = await _userManager.CreateAsync(user, "Nasiru@22");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["aasuccess"] = "Account created successfully";
                return Page();

            }
            return Page();
        }
    }
}
