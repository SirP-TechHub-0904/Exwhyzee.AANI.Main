using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]
    public class ContestantsModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        public ContestantsModel(AaniDbContext context, UserManager<Participant> userManager, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _storageService = storageService;
        }

        public IList<Campain> Campain { get; set; }
        public ExecutivePosition ExecutivePosition { get; set; }
        public long CurrentYearId { get; set; }
        [BindProperty]
        public IFormFile? ImageFile { get; set; }
        // For the "Add Contestant" modal
        [BindProperty]
        public InputModel Input { get; set; }
        public List<SelectListItem> QualifiedParticipants { get; set; }

        public class InputModel
        {
            [Required]
            public string ParticipantId { get; set; }
            public string Manifesto { get; set; }
            public string CampaignQoute { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(long yearId, long positionId)
        {
            CurrentYearId = yearId;
            ExecutivePosition = await _context.ExecutivePositions.FindAsync(positionId);

            if (ExecutivePosition == null) return NotFound();

            Campain = await _context.Campains
                .Include(c => c.ExecutivePosition)
                .Include(c => c.Participant).ThenInclude(x => x.SEC)
                .Where(x => x.ExecutivePositionId == positionId && x.OperationYearId == yearId)
                .ToListAsync();

            var existingCandidateIds = await _context.Campains
                .Where(c => c.OperationYearId == yearId)
                .Select(c => c.ParticipantId)
                .ToListAsync();

            // *** FIX: Step 1 - Fetch the raw data, including only the necessary related info ***
            var qualifiedUsers = await _userManager.Users
                .Include(u => u.SEC) // Include SEC for the year
                .Include(u => u.Chapter) // Include Chapter for the name
                .Where(u => !existingCandidateIds.Contains(u.Id))
                .OrderBy(u => u.Surname)
                .ToListAsync(); // Bring the data into memory

            // *** FIX: Step 2 - Format the in-memory list using the direct properties from Participant ***
            QualifiedParticipants = qualifiedUsers.Select(u =>
            {
                // Use the CurrentOffice property, with a fallback for when it's empty
                var office = !string.IsNullOrWhiteSpace(u.CurrentOccupation) ? u.CurrentOccupation : "N/A";

                var secInfo = u.SEC != null ? $"Sec {u.SEC.Year}" : "N/A";

                var chapterInfo = u.Chapter?.State ?? "N/A";

                return new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.Surname?.ToUpper()} {u.FirstName?.ToUpper()} {u.OtherName?.ToUpper()} ({secInfo} / {chapterInfo} / {office})"
                };
            }).ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostAddContestantAsync(long yearId, long positionId)
        {
            if (!ModelState.IsValid)
            {
                // If model state is invalid, just redirect back to the page
                return RedirectToPage(new { yearId, positionId });
            }

            var newCampaign = new Campain
            {
                ParticipantId = Input.ParticipantId,
                ExecutivePositionId = positionId,
                OperationYearId = yearId,
                Manifesto = Input.Manifesto,
                CampaignQoute = Input.CampaignQoute,
                Date = DateTime.UtcNow
            };
            // --- IMAGE UPLOAD LOGIC INTEGRATED HERE ---
            if (ImageFile != null)
            {
                await using var memoryStream = new MemoryStream();
                await ImageFile.CopyToAsync(memoryStream);

                var fileExt = Path.GetExtension(ImageFile.FileName);
                var docName = $"{Guid.NewGuid()}{fileExt}";

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
                if (xresult.Message.Contains("200"))
                {
                    newCampaign.ImageUrl = xresult.Url;
                    newCampaign.ImageKey = xresult.Key;
                }
                else
                {
                    TempData["error"] = "Unable to upload image.";
                }
            }
            _context.Campains.Add(newCampaign);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { yearId, positionId });
        }

        public async Task<IActionResult> OnPostRemoveAsync(long campaignId, long yearId, long positionId)
        {
            var campaignToRemove = await _context.Campains.FindAsync(campaignId);
            if (campaignToRemove != null)
            {
                _context.Campains.Remove(campaignToRemove);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { yearId, positionId });
        }


        // --- Existing Handlers for Modals ---
        public async Task<JsonResult> OnGetDetailsPicture(long id)
        {
            var user = await _context.Campains.Include(x => x.Participant).Include(x => x.ExecutivePosition).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return new JsonResult(null);

            var result = "<img class=\"img-fluid\" src=\"/img/campaign.jpg\">";
            if (user.ImageUrl != null)
            {
                result = "<img class=\"img-fluid\" src=\"" + user.ImageUrl + "\">";
            }

            var outcome = new ResultDto
            {
                Title = "CAMPAIGN PHOTO OF " + user.Participant.Fullname + " ON " + user.ExecutivePosition.Position,
                Description = result
            };
            return new JsonResult(outcome);
        }
        public async Task<JsonResult> OnGetDetails(long id)
        {
            var user = await _context.Campains.Include(x => x.Participant).Include(x => x.ExecutivePosition).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return new JsonResult(null);

            var result = user.Manifesto ?? "No manifesto provided.";

            var outcome = new ResultDto
            {
                Title = "MANIFESTO FOR " + user.Participant.Fullname + " ON " + user.ExecutivePosition.Position,
                Description = result
            };
            return new JsonResult(outcome);
        }
    }

    public class ResultDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}