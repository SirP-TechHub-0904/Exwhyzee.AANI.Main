using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]
    public class CreateModel : PageModel
    {
        private readonly AaniDbContext _context;

        public CreateModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? ElectionId { get; set; }

        [BindProperty]
        public ElectionInput Input { get; set; } = new ElectionInput();

        public Chapter? Chapter { get; set; }

        public class ElectionInput
        {
            public long Id { get; set; }

            [Required]
            [StringLength(250)]
            public string Title { get; set; } = string.Empty;

            [StringLength(2000)]
            public string? Description { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime StartAt { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime EndAt { get; set; }

            public ElectionStatus ElectionStatus { get; set; } 

        }

        public async Task<IActionResult> OnGetAsync(long chapterId, long? electionId)
        {
            ChapterId = chapterId;
            ElectionId = electionId;

            Chapter = await _context.Chapters.FindAsync(chapterId);
            if (Chapter == null) return NotFound();

            if (electionId.HasValue)
            {
                var e = await _context.ChapterElections.FirstOrDefaultAsync(x => x.Id == electionId.Value && x.ChapterId == chapterId);
                if (e == null) return NotFound();

                Input = new ElectionInput
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    // Convert UTC stored times to local for editing so the browser shows local time
                    StartAt = DateTime.SpecifyKind(e.StartAt, DateTimeKind.Utc).ToLocalTime(),
                    EndAt = DateTime.SpecifyKind(e.EndAt, DateTimeKind.Utc).ToLocalTime(), 
                    ElectionStatus = e.ElectionStatus
                };
            }
            else
            {
                // sensible defaults
                Input.StartAt = DateTime.Now.AddMinutes(5);
                Input.EndAt = DateTime.Now.AddHours(2);
                
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Chapter = await _context.Chapters.FindAsync(ChapterId);
            if (Chapter == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.EndAt <= Input.StartAt)
            {
                ModelState.AddModelError(string.Empty, "End time must be after start time.");
                return Page();
            }

            // Convert times from browser-local to UTC for storage
            var startUtc = DateTime.SpecifyKind(Input.StartAt, DateTimeKind.Local).ToUniversalTime();
            var endUtc = DateTime.SpecifyKind(Input.EndAt, DateTimeKind.Local).ToUniversalTime();

            if (Input.Id == 0)
            {
                var election = new Exwhyzee.AANI.Domain.Models.ChapterElection
                {
                    ChapterId = ChapterId,
                    Title = Input.Title,
                    Description = Input.Description,
                    StartAt = startUtc,
                    EndAt = endUtc,
                    ElectionStatus = Input.ElectionStatus
                };

                _context.ChapterElections.Add(election);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Election created.";
                // Redirect to Members page or to Manage Candidates — choose Members for next step
                return RedirectToPage("/ChapterElection/Elections", new { area = "Datapage", chapterId = ChapterId });
            }
            else
            {
                var existing = await _context.ChapterElections.FirstOrDefaultAsync(x => x.Id == Input.Id && x.ChapterId == ChapterId);
                if (existing == null) return NotFound();

                existing.Title = Input.Title;
                existing.Description = Input.Description;
                existing.StartAt = startUtc;
                existing.EndAt = endUtc;
                existing.ElectionStatus = Input.ElectionStatus;

                _context.ChapterElections.Update(existing);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Election updated.";
                return RedirectToPage("/ChapterElection/Elections", new { area = "Datapage", chapterId = ChapterId });
            }
        }
    }
}