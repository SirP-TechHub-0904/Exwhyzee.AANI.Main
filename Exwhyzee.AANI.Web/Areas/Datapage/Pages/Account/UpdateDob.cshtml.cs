using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Exwhyzee.AANI.Web.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class UpdateDobModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public UpdateDobModel(
            UserManager<Participant> userManager,
            AaniDbContext context,
            IConfiguration config,
            IStorageService storageService)
        {
            _userManager = userManager;

            _context = context;
            _config = config;
            _storageService = storageService;
        }

        [BindProperty]
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

        [BindProperty]
        public string Day { get; set; }
        [BindProperty]
        public string Month { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var updatedata = await _userManager.FindByIdAsync(Participant.Id);
            try
            {
                string dateString = Day + " " + Month + " 2000";
                updatedata.DOB = DateTime.ParseExact(dateString, "d M yyyy", CultureInfo.InvariantCulture);

            }
            catch (Exception c)
            {

            }
            updatedata.MessageTemplateCategoryId = Participant.MessageTemplateCategoryId;
            await _userManager.UpdateAsync(updatedata);
            TempData["success"] = "Successful";

            return RedirectToPage("/Account/MemberDetails", new { id = updatedata.Id, area="Datapage" });
        }
    }
}
