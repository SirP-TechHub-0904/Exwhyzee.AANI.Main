using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ExecutivePage.ActiveExecutive
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;

        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        [FromQuery]
        public long? OperationYearId { get; set; }

        public IList<Executive> Executive { get; set; }
        public OperationYear CurrentOperationYear { get; set; }

        // --- NEW: Property for the filter dropdown ---
        public SelectList OperationYearsSL { get; set; }

        public async Task OnGetAsync()
        {
            if (!OperationYearId.HasValue)
            {
                var activeYear = await _context.OperationYears.FirstOrDefaultAsync(oy => oy.IsActive);
                OperationYearId = activeYear?.Id;
            }

            // --- NEW: Populate the dropdown list ---
            // Fetch all years to use in the filter
            var allYears = await _context.OperationYears.OrderByDescending(oy => oy.StartDate).ToListAsync();
            // Create a SelectList, telling it which year is currently selected
            OperationYearsSL = new SelectList(allYears, "Id", "Name", OperationYearId);


            if (OperationYearId.HasValue)
            {
                CurrentOperationYear = await _context.OperationYears.FindAsync(OperationYearId.Value);

                Executive = await _context.Executives
                    .Include(e => e.ExecutivePosition)
                    .Include(e => e.Participant).ThenInclude(p => p.SEC)
                    .Where(e => e.OperationYearId == OperationYearId.Value)
                    .OrderBy(e => e.ExecutivePosition.SortOrder)
                    .ToListAsync();
            }
            else
            {
                Executive = new List<Executive>();
            }
        }

        // --- NEW DELETE HANDLER ---
        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var executive = await _context.Executives.FindAsync(id);

            if (executive != null)
            {
                // We need to know which year's page to redirect back to.
                var operationYearId = executive.OperationYearId;

                _context.Executives.Remove(executive);
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Executive removed successfully.";

                // Redirect back to the index page for the correct year.
                return RedirectToPage("./Index", new { OperationYearId = operationYearId });
            }

            // If the executive was not found, just redirect back to the main index.
            return RedirectToPage("./Index");
        }
    }
}