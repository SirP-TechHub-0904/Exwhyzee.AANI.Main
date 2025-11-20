using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Web.Helper.AWS;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class EditModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;


        public EditModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context,
            IConfiguration config,
            IStorageService storageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _config = config;
            _storageService = storageService;
        }
        [BindProperty]
        public IFormFile? imagefile { get; set; }

        [BindProperty]
        public Participant Participant { get; set; }

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

            ViewData["StateId"] = new SelectList(_context.States, "StateName", "StateName");

            var secs = _context.SECs.AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC " + x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");
            ViewData["ChapterId"] = new SelectList(_context.Chapters.OrderBy(x => x.State), "Id", "State");

            return Page();
        }
        public List<SelectListItem> LgaList { get; set; }

        public async Task<JsonResult> OnGetLGA(string id)
        {

            List<LocalGoverment> lga = new List<LocalGoverment>();

            var query = await _context.LocalGoverments
                .Include(x => x.States)
                .Where(x => x.States.StateName == id).ToListAsync();


            LgaList = query.Select(a =>
                                new SelectListItem
                                {
                                    Value = a.LGAName,
                                    Text = a.LGAName
                                }).ToList();
            return new JsonResult(LgaList);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var updateparticipant = await _userManager.FindByIdAsync(Participant.Id);

            if (imagefile != null)
            {
                try
                {
                    // Process file
                    await using var memoryStream = new MemoryStream();
                    await imagefile.CopyToAsync(memoryStream);

                    var fileExt = Path.GetExtension(imagefile.FileName);
                    var docName = $"{Guid.NewGuid()}{fileExt}";
                    // call server

                    var s3Obj = new Domain.Dtos.S3Object()
                    {
                        BucketName = "aani2023",
                        InputStream = memoryStream,
                        Name = docName
                    };

                    var cred = new AwsCredentials()
                    {
                        AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                        SecretKey = _config["AwsConfiguration:AWSSecretKey"]
                    };

                    var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, "");
                    // 
                    if (xresult.Message.Contains("200"))
                    {
                        updateparticipant.PictureUrl = xresult.Url;
                        updateparticipant.PictureKey = xresult.Key;
                    }
                    else
                    {
                        TempData["error"] = "unable to upload image";
                        //return Page();
                    }
                }
                catch (Exception c)
                {

                }
            }
            try
            {
                updateparticipant.Surname = Participant.Surname;
                updateparticipant.FirstName = Participant.FirstName;
                updateparticipant.OtherName = Participant.OtherName;
                updateparticipant.Title = Participant.Title;

                if (!String.IsNullOrEmpty(Participant.LGA))
                {
                    updateparticipant.LGA = Participant.LGA;
                }
                updateparticipant.State = Participant.State;
                updateparticipant.ContactAddress = Participant.ContactAddress;
                updateparticipant.HomeAddress = Participant.HomeAddress;
                updateparticipant.AltPhoneNumber = Participant.AltPhoneNumber;
                updateparticipant.PhoneNumber = Participant.PhoneNumber;
                updateparticipant.Sponsor = Participant.Sponsor;
                updateparticipant.GenderStatus = Participant.GenderStatus;
                updateparticipant.MaritalStatus = Participant.MaritalStatus;
                updateparticipant.ReligionStatus = Participant.ReligionStatus;
                updateparticipant.UserStatus = Participant.UserStatus;
                updateparticipant.MniStatus = Participant.MniStatus;
                updateparticipant.DOB = Participant.DOB;
                updateparticipant.SECId = Participant.SECId;
                updateparticipant.Bio = Participant.Bio;
                updateparticipant.ChapterId = Participant.ChapterId;
                updateparticipant.CurrentOccupation = Participant.CurrentOccupation;
                updateparticipant.CurrentWorkPlace = Participant.CurrentWorkPlace;
                updateparticipant.CurrentPosition = Participant.CurrentPosition;
                updateparticipant.EmergencyContactEmail = Participant.EmergencyContactEmail;
                updateparticipant.EmergencyContactPhone = Participant.EmergencyContactPhone;
                updateparticipant.EmergencyContactName = Participant.EmergencyContactName;
                try
                {
                    var email = await _userManager.GetEmailAsync(updateparticipant);
                    if (Participant.Email != email)
                    {


                        var currentEmail = await _userManager.GetEmailAsync(updateparticipant);
                        var currentUserName = await _userManager.GetUserNameAsync(updateparticipant);

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            TempData["aaerror"] = "New email cannot be empty.";
                        }


                        if (email == currentEmail && email == currentUserName)
                        {
                            TempData["aaerror"] = "Email/username is unchanged.";
                        }

                        // Ensure the new email is not used by another account
                        var existingByEmail = await _userManager.FindByEmailAsync(email);
                        if (existingByEmail != null && existingByEmail.Id != updateparticipant.Id)
                        {
                            TempData["aaerror"] = "Email is already used by another account.";
                        }

                        // Ensure the new username (we want username == email) is not used by another account
                        var existingByName = await _userManager.FindByNameAsync(email);
                        if (existingByName != null && existingByName.Id != updateparticipant.Id)
                        {
                            TempData["aaerror"] = "Username is already used by another account.";
                        }

                        // Generate token and change email
                        var emailToken = await _userManager.GenerateChangeEmailTokenAsync(updateparticipant, email);
                        var emailResult = await _userManager.ChangeEmailAsync(updateparticipant, email, emailToken);
                        if (!emailResult.Succeeded)
                        {
                            TempData["aaerror"] = "Error changing email: " + string.Join("; ", emailResult.Errors.Select(e => e.Description));
                        }

                        // Now update username to match email
                        var setUserNameResult = await _userManager.SetUserNameAsync(updateparticipant, email);
                        if (!setUserNameResult.Succeeded)
                        {
                            // Attempt to revert the email change back to the original to avoid partial state
                            var revertErrors = "";
                            try
                            {
                                var revertToken = await _userManager.GenerateChangeEmailTokenAsync(updateparticipant, currentEmail);
                                var revertResult = await _userManager.ChangeEmailAsync(updateparticipant, currentEmail, revertToken);
                                if (!revertResult.Succeeded)
                                {
                                    revertErrors = " Reverting email failed: " + string.Join("; ", revertResult.Errors.Select(e => e.Description));
                                }
                            }
                            catch
                            {
                                // ignore revert exceptions, but capture that revert may have failed
                                revertErrors = " Reverting email failed due to an unexpected error.";
                            }

                            TempData["aaerror"] = "Error changing username: " + string.Join("; ", setUserNameResult.Errors.Select(e => e.Description)) + revertErrors;
                        }
                        else
                        {

                            TempData["aasuccess"] = "Email Updated successfully";
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                await _userManager.UpdateAsync(updateparticipant);
                TempData["aasuccess"] = "Updated successfully";
            }
            catch (Exception)
            {
                ViewData["StateId"] = new SelectList(_context.States, "StateName", "StateName");

                var secs = _context.SECs.AsQueryable();
                var output = secs.Select(x => new SecDropdownListDto
                {
                    Id = x.Id,
                    SecYear = "SEC " + x.Number + " (" + x.Year + ")"
                });
                ViewData["SECId"] = new SelectList(output, "Id", "SecYear");

                TempData["aaerror"] = "Unable to update account";
                return Page();
            }

            return RedirectToPage("./Details", new { id = updateparticipant.Id });
        }


    }
}
