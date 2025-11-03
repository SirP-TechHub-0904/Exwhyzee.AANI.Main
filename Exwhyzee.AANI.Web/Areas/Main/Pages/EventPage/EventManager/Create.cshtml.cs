using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
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

        public CreateModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; }

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

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Event created successfully";

            // Redirect back to the Index page for the specific year we were working on.
            return RedirectToPage("./Index", new { OperationYearId = Event.OperationYearId });
        }
    }
}