using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PaperPage.Category
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        public CreateModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _config = config;
            _storageService = storageService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PaperCategory PaperCategory { get; set; }

        [BindProperty]
        public IFormFile? imagefile { get; set; }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
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
                            PaperCategory.CoverUrl = xresult.Url;
                            PaperCategory.CoverKey = xresult.Key;
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


                _context.paperCategories.Add(PaperCategory);
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Updated successfully";
            }
            catch (Exception)
            {
                TempData["aaerror"] = "Unable to Add Contributors";
            }
            return RedirectToPage("./Index");
        }
    }
}
