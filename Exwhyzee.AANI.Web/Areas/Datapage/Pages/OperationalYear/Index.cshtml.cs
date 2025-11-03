using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.OperationalYear
{
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;

        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        public IList<OperationYear> OperationYear { get; set; }

        [BindProperty]
        public OperationYear Input { get; set; }

        public async Task OnGetAsync()
        {
            OperationYear = await _context.OperationYears.OrderByDescending(oy => oy.StartDate).ToListAsync();
        }

        // Handler for the "Add New" modal submission
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                // In a real app, you might return a special error result for the client-side script to handle
                return RedirectToPage();
            }
            if (Input.IsActive)
            {
                var currentlyActive = await _context.OperationYears.FirstOrDefaultAsync(oy => oy.IsActive);
                if (currentlyActive != null)
                {
                    currentlyActive.IsActive = false;
                }
            }
            await _context.OperationYears.AddAsync(Input);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // Handler to get the data for the "Edit" modal
        public async Task<JsonResult> OnGetJsonAsync(long id)
        {
            var year = await _context.OperationYears.FindAsync(id);
            if (year == null) return new JsonResult(null);

            // Return all properties, including the new IsActive flag
            return new JsonResult(new
            {
                id = year.Id,
                name = year.Name,
                startDate = year.StartDate.ToString("yyyy-MM-dd"),
                endDate = year.EndDate.ToString("yyyy-MM-dd"),
                isActive = year.IsActive
            });
        }

        // Handler for the "Edit" modal submission
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }
            // If the edited year is being set to active, deactivate any other active year.
            if (Input.IsActive)
            {
                // Find any other year that is active (but not the one we are currently editing)
                var currentlyActive = await _context.OperationYears
                    .FirstOrDefaultAsync(oy => oy.IsActive && oy.Id != Input.Id);

                if (currentlyActive != null)
                {
                    currentlyActive.IsActive = false;
                }
            }

            _context.Attach(Input).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // Handler for the "Delete" modal submission
        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var year = await _context.OperationYears.FindAsync(id);
            if (year != null)
            {
                _context.OperationYears.Remove(year);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}