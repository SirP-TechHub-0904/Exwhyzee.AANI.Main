using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.EventManager
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CreateModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        public CreateModel(AaniDbContext context, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _config = config;
            _storageService = storageService;
        }

        [BindProperty]
        public Event Event { get; set; }
        [BindProperty]
        public IFormFile? imagefile { get; set; }

        public OperationYear CurrentOperationYear { get; set; }

        // This method now receives the OperationYearId from the URL
        public async Task<IActionResult> OnGetAsync(long operationYearId)
        {
            CurrentOperationYear = await _context.OperationYears.FindAsync(operationYearId);
            if (CurrentOperationYear == null)
            {
                // Can't create an event for a year that doesn't exist.
                return NotFound();
            }

            // Pre-set the OperationYearId for the new event.
            Event = new Event { OperationYearId = operationYearId };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    // If the model is invalid, we must repopulate the year info before showing the page again.
            //    CurrentOperationYear = await _context.OperationYears.FindAsync(Event.OperationYearId);
            //    return Page();
            //}
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
                        Event.ImageUrl = xresult.Url;
                        Event.ImageKey = xresult.Key;
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

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Event created successfully";

            // Redirect back to the Index page for the specific year we were working on.
            return RedirectToPage("./Index", new { OperationYearId = Event.OperationYearId });
        }
    }
}