using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class UpdateIDCardModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;


        public UpdateIDCardModel(
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

 
            var secs = _context.SECs.AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC " + x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");
 
            return Page();
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

                
                updateparticipant.GenderStatus = Participant.GenderStatus; 
                updateparticipant.SECId = Participant.SECId; 

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

            return RedirectToPage("./IDCardQrCode", new {id = updateparticipant.Id});
        }


    }

}
