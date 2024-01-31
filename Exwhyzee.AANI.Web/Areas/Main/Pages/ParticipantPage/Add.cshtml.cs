using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.AANI.Domain.Dtos;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Exwhyzee.AANI.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Web.Helper.AWS;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class AddModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly ILogger<CreateModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        public AddModel(
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
        public IActionResult OnGet()
        {

           
            var secs = _context.SECs.AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC " + x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");

            return Page();
        }

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
            public string? Email { get; set; }

            public string? Surname { get; set; }
            [Display(Name = "First Name")]
            public string? FirstName { get; set; }
            [Display(Name = "Other Name")]
            public string? OtherName { get; set; }
            public string? Title { get; set; }
            public string? DOB { get; set; }

            public string? Sponsor { get; set; }
            public string? PhoneNumber { get; set; }
            public long SECId { get; set; }
            public GenderStatus GenderStatus { get; set; }


        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            string imgKey = string.Empty;
            string imgUrl = string.Empty;
            //image
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
                        imgUrl = xresult.Url;
                        imgKey = xresult.Key;
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
            var user = new Participant
            {
                UserName = Input.Email,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                FirstName = Input.FirstName,
                Surname = Input.Surname,
                OtherName = Input.OtherName,
                Title = Input.Title,
                Sponsor = Input.Sponsor,
                SECId = Input.SECId,
                GenderStatus = Input.GenderStatus,
                MniStatus = MniStatus.MNI,
                AliveStatus = AliveStatus.Alive,
                VerificationStatus = VerificationStatus.NONE,
                ActiveStatus = ActiveStatus.NONE,
                UserStatus = UserStatus.MNI,
                PictureKey = imgKey,
                PictureUrl = imgUrl,
            };
            Guid pass = Guid.NewGuid();
            var result = await _userManager.CreateAsync(user, pass.ToString().Replace("-", ".") + "XY");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "MNI");
                TempData["aasuccess"] = "Account created successfully for "+user.Fullname;
                TempData["aasssuccess"] = "Account created successfully for "+user.Fullname;
                return RedirectToPage("./Details", new { id = user.Id });
            }
            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, error.Description);
            //}
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            TempData["aaerror"] = messages;
            //
            var secs = _context.SECs.AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC " + x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");

            return Page();

        }


    }
}
