using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class UpdateImageModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        private readonly AaniDbContext _context;

        public UpdateImageModel(
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
        public IFormFile? imagefile { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var updatedata = await _userManager.FindByIdAsync(Participant.Id);
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

                    var s3Obj = new S3Object()
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
                        updatedata.PictureUrl = xresult.Url;
                        updatedata.PictureKey = xresult.Key;
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

            await _userManager.UpdateAsync(updatedata);
            TempData["success"] = "Successful";

            return RedirectToPage("/Account/MemberDetails", new { id = updatedata.Id, area = "Datapage" });
        }
    }
}
