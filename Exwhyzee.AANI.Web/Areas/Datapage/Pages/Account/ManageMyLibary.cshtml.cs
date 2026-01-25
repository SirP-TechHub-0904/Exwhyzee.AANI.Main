using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ManageMyLibaryModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;

        public ManageMyLibaryModel(AaniDbContext context, UserManager<Participant> userManager, IStorageService storageService, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _storageService = storageService;
            _config = config;
        }

        public IList<Paper> MyPapers { get; set; }

        [BindProperty]
        public Paper NewPaper { get; set; }

        [BindProperty]
        public IFormFile? imagefile { get; set; }


        [BindProperty]
        public IFormFile? docfile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            await LoadData(user.Id);
            return Page();
        }

        private async Task LoadData(string userId)
        {
            MyPapers = await _context.Papers
                .Include(p => p.PaperCategory)
                .Where(p => p.ParticipantId == userId)
                .OrderByDescending(p => p.Year)
                .ToListAsync();

            ViewData["PaperCategoryId"] = new SelectList(await _context.paperCategories.ToListAsync(), "Id", "Title");
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var paper = await _context.Papers.FindAsync(id);
            if (paper != null)
            {
                _context.Papers.Remove(paper);
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Deleted successfully";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            NewPaper.ParticipantId = user.Id;

            if (imagefile != null)
            {
                // S3 Upload Logic
                await using var memoryStream = new MemoryStream();
                await imagefile.CopyToAsync(memoryStream);
                var docName = $"{Guid.NewGuid()}{Path.GetExtension(imagefile.FileName)}";

                var s3Obj = new S3Object { BucketName = "aani2023", InputStream = memoryStream, Name = docName };
                var cred = new AwsCredentials
                {
                    AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                    SecretKey = _config["AwsConfiguration:AWSSecretKey"]
                };

                var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, "");
                if (xresult.Message.Contains("200"))
                {
                    NewPaper.CoverUrl = xresult.Url;
                    NewPaper.CoverKey = xresult.Key;
                }
            }
            if (docfile != null)
            {
                // S3 Upload Logic
                await using var memoryStream = new MemoryStream();
                await docfile.CopyToAsync(memoryStream);
                var docName = $"{Guid.NewGuid()}{Path.GetExtension(docfile.FileName)}";

                var s3Obj = new S3Object { BucketName = "aani2023", InputStream = memoryStream, Name = docName };
                var cred = new AwsCredentials
                {
                    AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                    SecretKey = _config["AwsConfiguration:AWSSecretKey"]
                };

                var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, "");
                if (xresult.Message.Contains("200"))
                {
                    NewPaper.FileUrl = xresult.Url;
                    NewPaper.FileKey = xresult.Key;
                }
            }
            _context.Papers.Add(NewPaper);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Added to library";

            return RedirectToPage();
        }
    }
}