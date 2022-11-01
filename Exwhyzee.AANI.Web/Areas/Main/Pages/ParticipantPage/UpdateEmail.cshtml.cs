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
            var updateparticipant = await _userManager.FindByIdAsync(Participant.Id);

            var email = await _userManager.GetEmailAsync(updateparticipant);
            if (NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(updateparticipant);
                var code = await _userManager.GenerateChangeEmailTokenAsync(updateparticipant, NewEmail);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                //var xcode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ChangeEmailAsync(updateparticipant, NewEmail, code);
                if (!result.Succeeded)
                {
                    TempData["aaerror"] = "Error changing email.";
                    return Page();
                }
                TempData["aasuccess"] = "Email Updated successfully";
                return RedirectToPage("./Details", new { id = Participant.Id });
            }
            TempData["aaerror"] = "Error changing email or Email is already used.";


            return RedirectToPage("./Details", new { id = Participant.Id });
        }

    }
}
