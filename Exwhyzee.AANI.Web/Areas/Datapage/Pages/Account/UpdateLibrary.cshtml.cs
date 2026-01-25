using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    public class UpdateLibraryModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;

        public UpdateLibraryModel(AaniDbContext context, IStorageService storageService, IConfiguration config)
        {
            _context = context;
            _storageService = storageService;
            _config = config;
        }

        [BindProperty]
        public Paper Paper { get; set; }

        [BindProperty]
        public IFormFile? imagefile { get; set; } // Cover Image Input

        [BindProperty]
        public IFormFile? paperfile { get; set; } // PDF/Document Input

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Paper = await _context.Papers
                .Include(p => p.PaperCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Paper == null) return NotFound();

            // Load Categories for the dropdown
            ViewData["PaperCategoryId"] = new SelectList(await _context.paperCategories.ToListAsync(), "Id", "Title");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var paperToUpdate = await _context.Papers.FirstOrDefaultAsync(x => x.Id == Paper.Id);
            if (paperToUpdate == null) return NotFound();

            // 1. Update Standard Fields
            paperToUpdate.Title = Paper.Title;
            paperToUpdate.Description = Paper.Description;
            paperToUpdate.Year = Paper.Year;
            paperToUpdate.PaperCategoryId = Paper.PaperCategoryId;

            // AWS Credentials
            var cred = new AwsCredentials
            {
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"]
            };

            // 2. Handle Cover Image Upload (Optional)
            if (imagefile != null)
            {
                var coverResult = await UploadFileAsync(imagefile, cred, "library/covers");
                paperToUpdate.CoverUrl = coverResult.url;
                paperToUpdate.CoverKey = coverResult.key;
            }

            // 3. Handle Main File/Document Upload (Optional)
            if (paperfile != null)
            {
                var fileResult = await UploadFileAsync(paperfile, cred, "library/documents");
                paperToUpdate.FileUrl = fileResult.url;
                paperToUpdate.FileKey = fileResult.key;
            }

            _context.Attach(paperToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["aasuccess"] = "Paper updated successfully";
            return RedirectToPage("./ManageMyLibary");
        }

        private async Task<(string url, string key)> UploadFileAsync(IFormFile file, AwsCredentials cred, string folder)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var s3Obj = new S3Object
            {
                BucketName = "aani2023",
                InputStream = memoryStream,
                Name = $"{folder}/{uniqueName}"
            };

            var result = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, "");
            return (result.Url, result.Key);
        }
    }
}