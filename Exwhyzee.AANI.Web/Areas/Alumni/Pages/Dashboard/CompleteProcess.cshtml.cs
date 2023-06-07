using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Entity;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
        public class CompleteProcessModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CompleteProcessModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [BindProperty]
        public Participant Participant { get; set; }
        public SEC SEC { get; set; }

        [BindProperty]
        public string Day { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Month { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {

            Participant = await _userManager.FindByIdAsync(id);
            if(Participant == null)
            {
                return RedirectToPage("./Verify");
            } 
            SEC = _context.SECs.FirstOrDefault(x => x.Id == Participant.SECId);
            ViewData["StateId"] = new SelectList(_context.States, "StateName", "StateName");

            return Page();
        }

 
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var updateparticipant = await _userManager.FindByIdAsync(Participant.Id);
                if (updateparticipant != null)
                {
                    updateparticipant.DateUpdated = DateTime.UtcNow.AddHours(1);
                    updateparticipant.Email = Email;
                    updateparticipant.PhoneNumber = Phone;
                    updateparticipant.DOB = Day + "/" + Month + "/" + 2000.ToString();
                    updateparticipant.GenderStatus = Participant.GenderStatus;
                    updateparticipant.State = Participant.State;
                    updateparticipant.CurrentOffice = Participant.CurrentOffice;
                    updateparticipant.CurrentPosition = Participant.CurrentPosition;
                    updateparticipant.VerificationStatus = VerificationStatus.Awaiting;

                    try
                    {
                        var chapter = _context.Chapters.AsQueryable();
                        var chapterid = chapter.Where(x => x.State.ToLower().Contains(updateparticipant.State.ToLower())).ToList();

                        if (chapterid.FirstOrDefault() != null)
                        {
                            updateparticipant.ChapterId = chapterid.FirstOrDefault().Id;
                        }

                    }
                    catch (Exception u)
                    {

                    }
                    await _userManager.UpdateAsync(updateparticipant);
                    await _userManager.UpdateNormalizedEmailAsync(updateparticipant);

                    



                    TempData["response"] = "Your Submission is Under Review. You will get a call and an email. Once your Data is verified. Thanks";
                    return RedirectToPage("./Response");
                }
            }catch(Exception c)
            {

            }

            TempData["error"] = "something happened. unable to continue. try again";
            return RedirectToPage("./Verify");
        }
 

    }

}
