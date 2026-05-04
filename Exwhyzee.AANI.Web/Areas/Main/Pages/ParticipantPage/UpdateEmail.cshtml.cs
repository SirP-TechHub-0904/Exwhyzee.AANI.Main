using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class UpdateEmailModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;

        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public UpdateEmailModel(
            UserManager<Participant> userManager,
            AaniDbContext context)
        {
            _userManager = userManager;

            _context = context;
        }

        [BindProperty]
        public Participant Participant { get; set; }

        [BindProperty]
        public string NewEmail { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }

            Participant = await _userManager.FindByIdAsync(id);

            if (Participant == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }


            return Page();
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (Participant == null || string.IsNullOrWhiteSpace(Participant.Id))
            {
                TempData["aaerror"] = "Invalid participant.";
                return RedirectToPage("./Index");
            }

            var user = await _userManager.FindByIdAsync(Participant.Id);
            if (user == null)
            {
                TempData["aaerror"] = "User not found.";
                return RedirectToPage("./Index");
            }

            var currentEmail = await _userManager.GetEmailAsync(user);
            var currentUserName = await _userManager.GetUserNameAsync(user);

            var newValue = NewEmail?.Trim();
            if (string.IsNullOrWhiteSpace(newValue))
            {
                TempData["aaerror"] = "Email is required.";
                return RedirectToPage("./Details", new { id = Participant.Id });
            }

            // No change
            if (string.Equals(newValue, currentEmail, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(newValue, currentUserName, StringComparison.OrdinalIgnoreCase))
            {
                TempData["aaerror"] = "No changes detected.";
                return RedirectToPage("./Details", new { id = Participant.Id });
            }

            // Check email uniqueness
            var emailOwner = await _userManager.FindByEmailAsync(newValue);
            if (emailOwner != null && emailOwner.Id != user.Id)
            {
                TempData["aaerror"] = "Email is already used.";
                return RedirectToPage("./Details", new { id = Participant.Id });
            }

            // Check username uniqueness
            var userNameOwner = await _userManager.FindByNameAsync(newValue);
            if (userNameOwner != null && userNameOwner.Id != user.Id)
            {
                TempData["aaerror"] = "Username is already used.";
                return RedirectToPage("./Details", new { id = Participant.Id });
            }

            // Since email and username must always be the same, update both
            // Option 1: direct set methods
            var setEmailResult = await _userManager.SetEmailAsync(user, newValue);
            if (!setEmailResult.Succeeded)
            {
                TempData["aaerror"] = string.Join(" | ", setEmailResult.Errors.Select(e => e.Description));
                return RedirectToPage("./Details", new { id = Participant.Id });
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(user, newValue);
            if (!setUserNameResult.Succeeded)
            {
                TempData["aaerror"] = string.Join(" | ", setUserNameResult.Errors.Select(e => e.Description));
                return RedirectToPage("./Details", new { id = Participant.Id });
            }

            // Optional: if your app requires email confirmed after change, reset it
            user.EmailConfirmed = true; // or false, depending on your flow

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                TempData["aaerror"] = string.Join(" | ", updateResult.Errors.Select(e => e.Description));
                return RedirectToPage("./Details", new { id = Participant.Id });
            }

            TempData["aasuccess"] = "Email and username updated successfully.";
            return RedirectToPage("./Details", new { id = Participant.Id });
        }
    }
}
