using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;

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
        public InputModel Input { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Day { get; set; }
            [Required]
            public string Phone { get; set; }
            [Required]
            public string Month { get; set; }

        }

        public async Task<IActionResult> OnGet(string id)
        {

            Participant = await _userManager.FindByIdAsync(id);
            if (Participant == null)
            {
                return RedirectToPage("./Verify");
            }
            SEC = _context.SECs.FirstOrDefault(x => x.Id == Participant.SECId);
            ViewData["StateId"] = new SelectList(_context.States.OrderBy(x => x.StateName), "StateName", "StateName");
            ViewData["ChapterId"] = new SelectList(_context.Chapters.OrderBy(x => x.State), "Id", "State");

            return Page();
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            DateTime dateTime = DateTime.UtcNow;
            try
            {
                string dateString = Input.Day + " " + Input.Month + " 2000";
                dateTime = DateTime.ParseExact(dateString, "d M yyyy", CultureInfo.InvariantCulture);

            }
            catch (Exception c)
            {

            }
            try
            {
                var updateparticipant = await _userManager.FindByIdAsync(Participant.Id);
                if (updateparticipant != null)
                {
                    updateparticipant.DateUpdated = DateTime.UtcNow.AddHours(1);
                    updateparticipant.Email = Input.Email;
                    updateparticipant.PhoneNumber = Input.Phone;
                    updateparticipant.DOB = dateTime;
                    updateparticipant.GenderStatus = Participant.GenderStatus;
                    updateparticipant.State = Participant.State;
                    updateparticipant.CurrentOffice = Participant.CurrentOffice.ToUpper();
                    updateparticipant.CurrentPosition = Participant.CurrentPosition.ToUpper();
                    updateparticipant.VerificationStatus = VerificationStatus.Awaiting;
                    updateparticipant.ChapterId = Participant.ChapterId;

                   var update = await _userManager.UpdateAsync(updateparticipant);
                    if (update.Succeeded)
                    { 
                        await _userManager.UpdateNormalizedEmailAsync(updateparticipant);
                       
                        var check = await _userManager.RemovePasswordAsync(updateparticipant);
                        if (check.Succeeded)
                        {
                            var resolve = await _userManager.AddPasswordAsync(updateparticipant, Input.Password);
                            if (resolve.Succeeded)
                            {
                                TempData["login"] = "login";
                                TempData["response"] = "Your Upate has been Successfully Submitted. <br>Kindly click the button below to login with your email address and password.";
                                return RedirectToPage("./Response");
                            }


                        }

                    }
                    foreach (var error in update.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    SEC = _context.SECs.FirstOrDefault(x => x.Id == Participant.SECId);
                    ViewData["StateId"] = new SelectList(_context.States.OrderBy(x => x.StateName), "StateName", "StateName");
                    ViewData["ChapterId"] = new SelectList(_context.Chapters.OrderBy(x => x.State), "Id", "State");
                    return Page();
                }
            }
            catch (Exception c)
            {
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
            }

            TempData["error"] = "something happened. unable to continue. try again";
            return RedirectToPage("./Verify");
        }


    }

}
