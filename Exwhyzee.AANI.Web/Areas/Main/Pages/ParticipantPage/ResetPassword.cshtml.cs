using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly SignInManager<Participant> _signInManager;
        private readonly ILogger<ResetPasswordModel> _logger;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;


        public ResetPasswordModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            ILogger<ResetPasswordModel> logger, Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {


            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }


        }

        [BindProperty]
        public string UIDD { get; set; }

        [BindProperty]
        public string PIX { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            UIDD = user.Id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UIDD);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var changePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!changePasswordResult.Succeeded)
            {

                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                TempData["aaerror"] = messages;
                return Page();
            }
            var NewchangePasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (NewchangePasswordResult.Succeeded)
            {

                _logger.LogInformation("User changed their password successfully.");

                TempData["aasuccess"] = "Password updated successfully";
                return RedirectToPage("./ResetPassword", new { id = user.Id });
            }
            TempData["aaerror"] = "Unable to reset Password";
            return RedirectToPage("./ResetPassword", new { id = user.Id });

        }
    }

}
