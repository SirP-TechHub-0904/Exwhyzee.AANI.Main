using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    public class UpdateChapterAccountModel : PageModel
    {
        private readonly Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public UpdateChapterAccountModel(Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public List<SelectListItem> ParticipantList { get; set; } = new List<SelectListItem>();
        public List<ChapterExecutive> ChapterExecutives { get; set; } = new List<ChapterExecutive>();
        public long CurrentChapterId { get; set; }

        public class InputModel
        {
            // For the edit modal
            public long Id { get; set; }

            // For the add form
            [Required(ErrorMessage = "Please select a member.")]
            public string ParticipantId { get; set; }

            [Required(ErrorMessage = "Position cannot be empty.")]
            public string Position { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(long chapterId)
        {
            CurrentChapterId = chapterId;
            // Load available participants for dropdown
            ParticipantList = _userManager.Users
    .Where(x => x.ChapterId == chapterId)
    .ToList() // Materialize the query first
    .Select(x => new SelectListItem
    {
        Value = x.Id.ToString(),
        Text = $"{x.Surname} {x.FirstName} {x.OtherName}"
    })
    .OrderBy(x => x.Text)
    .ToList();

            // Load current chapter executives
            ChapterExecutives = await _context.ChapterExecutives
                .Where(x => x.ChapterId == chapterId)
                .Include(x => x.Participant)
                .OrderBy(x => x.Participant.Surname)
                .ThenBy(x => x.Participant.FirstName)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long chapterId)
        {
            if (!ModelState.IsValid)
            {
                // If the main form is invalid, reload the page data
                await OnGetAsync(chapterId);
                return Page();
            }

            // Add or update executive based on the main form (top of the page)
            var exec = await _context.ChapterExecutives
                .FirstOrDefaultAsync(x => x.ParticipantId == Input.ParticipantId && x.ChapterId == chapterId);

            if (exec == null)
            {
                exec = new ChapterExecutive
                {
                    ParticipantId = Input.ParticipantId,
                    Position = Input.Position,
                    ChapterId = chapterId
                };
                _context.ChapterExecutives.Add(exec);
            }
            else
            {
                // If participant is already an exec, update their position
                exec.Position = Input.Position;
                _context.ChapterExecutives.Update(exec);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage(new { chapterId });
        }

        // Handles the submission from the EDIT modal
        public async Task<IActionResult> OnPostUpdatePositionAsync(long chapterId)
        {
            var execToUpdate = await _context.ChapterExecutives.FindAsync(Input.Id);
            if (execToUpdate != null && !string.IsNullOrWhiteSpace(Input.Position))
            {
                execToUpdate.Position = Input.Position;
                _context.ChapterExecutives.Update(execToUpdate);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { chapterId });
        }

        // Handles the submission from the DELETE modal
        public async Task<IActionResult> OnPostDeleteAsync(long id, long chapterId)
        {
            var execToDelete = await _context.ChapterExecutives.FindAsync(id);
            if (execToDelete != null)
            {
                _context.ChapterExecutives.Remove(execToDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { chapterId });
        }
    }
}