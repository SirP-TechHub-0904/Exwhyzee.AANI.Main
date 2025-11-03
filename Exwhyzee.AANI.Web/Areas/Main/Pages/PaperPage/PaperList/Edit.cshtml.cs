using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PaperPage.PaperList
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class EditModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        public EditModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _storageService = storageService;
        }

        [BindProperty]
        public Paper Paper { get; set; }

        [BindProperty]
        public IFormFile? imagefile { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Paper = await _context.Papers
                .Include(p => p.Event)
                .Include(p => p.PaperCategory)
                .Include(p => p.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (Paper == null)
            {
                return NotFound();
            }
           ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title");
           ViewData["PaperCategoryId"] = new SelectList(_context.paperCategories, "Id", "Title");
            var partaccount = _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).AsQueryable();
            var secoutput = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname =  x.Surname + " " + x.FirstName + " " + x.OtherName + "(SEC " + x.SEC.Number + "- " + x.SEC.Year + ")"
            });
            ViewData["ParticipantId"] = new SelectList(secoutput, "Id", "Fullname"); return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
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
                        Paper.CoverUrl = xresult.Url;
                        Paper.CoverKey = xresult.Key;
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


            _context.Attach(Paper).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Updated successfully";

            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["aaerror"] = "Unable to update Paper";

                var partaccount = _userManager.Users.Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).AsQueryable();
                var output = partaccount.Select(x => new ParticipantDropdownDto
                {
                    Id = x.Id,
                    Fullname =  x.Surname + " " + x.FirstName + " " + x.OtherName
                });
                ViewData["ParticipantId"] = new SelectList(output, "Id", "Fullname");
                return Page();
            }

            return RedirectToPage("/PaperPage/Category/Details", new {id = Paper.PaperCategoryId});
        }

        private bool PaperExists(long id)
        {
            return _context.Papers.Any(e => e.Id == id);
        }
    }
}
