using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ExecutivePage.ActiveExecutive
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public CreateModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Executive Executive { get; set; }

        public OperationYear CurrentOperationYear { get; set; }

        // This property receives the OperationYearId from the URL
        [FromQuery]
        public long OperationYearId { get; set; }

        public async Task<IActionResult> OnGetAsync(long? operationYearId)
        {
            CurrentOperationYear = await _context.OperationYears.FindAsync(operationYearId);
            if (CurrentOperationYear == null)
            {
                return NotFound("The specified Operation Year does not exist.");
            }

            Executive = new Executive { OperationYearId = operationYearId };

            // --- START OF CHANGE ---

            // 1. Get the IDs of participants who are already executives for THIS year.
            var existingExecutiveIds = await _context.Executives
                .Where(e => e.OperationYearId == operationYearId)
                .Select(e => e.ParticipantId)
                .ToListAsync();

            // 2. Fetch all MNI participants, but EXCLUDE those who are already executives this year.
            var availableParticipants = await _userManager.Users
                .Include(x => x.SEC)
                .Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI &&
                            !existingExecutiveIds.Contains(x.Id)) // This line filters out existing executives
                .OrderBy(x => x.Surname)
                .ToListAsync();

            // --- END OF CHANGE ---

            var secoutput = availableParticipants.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname = $"{x.Surname} {x.FirstName} {x.OtherName} (SEC {x.SEC.Number}-{x.SEC.Year})"
            });

            ViewData["ParticipantId"] = new SelectList(secoutput, "Id", "Fullname");
            //ViewData["ExecutivePositionId"] = new SelectList(_context.ExecutivePositions.OrderBy(p => p.SortOrder), "Id", "Position");

            // --- NEW: Filter POSITIONS ---
            // 1. Get the IDs of positions that are already taken for THIS year.
            var takenPositionIds = await _context.Executives
                .Where(e => e.OperationYearId == operationYearId)
                .Select(e => e.ExecutivePositionId)
                .ToListAsync();

            // 2. Fetch all positions, but EXCLUDE those that are already taken.
            var availablePositions = await _context.ExecutivePositions
                .Where(p => !takenPositionIds.Contains(p.Id))
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            // 3. Create the dropdown list from only the available positions.
            ViewData["ExecutivePositionId"] = new SelectList(availablePositions, "Id", "Position");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    // If the model is invalid, we need to repopulate the dropdowns and show the page again.
            //    await OnGetAsync(Executive.OperationYearId);
            //    return Page();
            //}
            // --- Final check to prevent race conditions ---
            var isPositionTaken = await _context.Executives.AnyAsync(e =>
                e.OperationYearId == Executive.OperationYearId &&
                e.ExecutivePositionId == Executive.ExecutivePositionId);

            if (isPositionTaken)
            {
                ModelState.AddModelError("Executive.ExecutivePositionId", "This position has already been filled for this year.");
                await OnGetAsync(Executive.OperationYearId);
                return Page();
            }

            _context.Executives.Add(Executive);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Executive created successfully";

            // Redirect back to the Index page for the specific year we were working on.
            return RedirectToPage("./Index", new { OperationYearId = Executive.OperationYearId });
        }
    }
}