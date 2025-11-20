using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SettingsPage
{
    public class OtherSettingsModel : PageModel
    {

        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public OtherSettingsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _config = config;
            _storageService = storageService;
        }
        [BindProperty]
        public ContactSettingsModel ContactSettingsModel { get; set; }
        [BindProperty]
        public IFormFile? imagefile { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {

            ContactSettingsModel = await _context.ContactSettings
                .FirstOrDefaultAsync();

            if (ContactSettingsModel == null)
            {
                return NotFound();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            var updateSetting = await _context.ContactSettings
                .FirstOrDefaultAsync();
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
                        updateSetting.BreadcrumImageUrl = xresult.Url;
                        updateSetting.BreadcrumImageId = xresult.Key;
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

            updateSetting.EventTitle = ContactSettingsModel.EventTitle;
            updateSetting.EventSubtitle = ContactSettingsModel.EventSubtitle;
            updateSetting.EventDescription = ContactSettingsModel.EventDescription;
            updateSetting.ExecutiveTitle = ContactSettingsModel.ExecutiveTitle;
            updateSetting.ExecutiveSubtitle = ContactSettingsModel.ExecutiveSubtitle;
            updateSetting.ExecutiveDescription = ContactSettingsModel.ExecutiveDescription;
            updateSetting.BlogTitle = ContactSettingsModel.BlogTitle;
            updateSetting.BlogSubtitle = ContactSettingsModel.BlogSubtitle;
            updateSetting.BlogDescription = ContactSettingsModel.BlogDescription;

            _context.ContactSettings.Update(updateSetting); 
            await _context.SaveChangesAsync();
            TempData["success"] = "Successful";

            return RedirectToPage("./OtherSettings");
        }

         
    }
}
