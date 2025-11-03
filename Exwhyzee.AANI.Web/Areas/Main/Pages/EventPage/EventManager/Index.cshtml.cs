using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.EventManager
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;

        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        // --- Properties for the new OperationYear filter ---
        [FromQuery]
        public long? OperationYearId { get; set; }
        public OperationYear CurrentOperationYear { get; set; }
        public SelectList OperationYearsSL { get; set; }

        public IList<Event> Event { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Handle OperationYear filter (defaults to active year)
            if (!OperationYearId.HasValue)
            {
                var activeYear = await _context.OperationYears.FirstOrDefaultAsync(oy => oy.IsActive);
                OperationYearId = activeYear?.Id;
            }

            // Populate the year dropdown
            var allYears = await _context.OperationYears.OrderByDescending(oy => oy.StartDate).ToListAsync();
            OperationYearsSL = new SelectList(allYears, "Id", "Name", OperationYearId);

            if (OperationYearId.HasValue)
            {
                CurrentOperationYear = await _context.OperationYears.FindAsync(OperationYearId.Value);
            }
            else
            {
                // If no years exist at all, create an empty list and stop.
                Event = new List<Event>();
                return;
            }

            // 2. Build the main query, filtering by the selected OperationYearId
            Event = await _context.Events
                .Where(e => e.OperationYearId == OperationYearId.Value)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }
    }
}