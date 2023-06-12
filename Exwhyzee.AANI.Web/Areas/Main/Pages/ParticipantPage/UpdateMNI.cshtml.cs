using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    public class UpdateMNIModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;



        public UpdateMNIModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }

           var Participant = await _userManager.FindByIdAsync(id);

            if (Participant == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }
            if(Participant.MniStatus == Domain.Enums.MniStatus.NONE)
            {
                Participant.MniStatus = Domain.Enums.MniStatus.MNI;
            }else if (Participant.MniStatus == Domain.Enums.MniStatus.MNI)
            {
                Participant.MniStatus = Domain.Enums.MniStatus.NONE;
            }
            await _userManager.UpdateAsync(Participant);
            TempData["aasuccess"] = "Updated successfully";
            return Page();
        }

    }
}
