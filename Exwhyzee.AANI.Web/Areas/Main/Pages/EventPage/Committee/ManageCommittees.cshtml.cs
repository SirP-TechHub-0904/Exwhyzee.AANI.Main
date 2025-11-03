using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.Committee
{
    [Authorize(Roles = "Admin")]
    public class ManageCommitteesModel : PageModel
    {
        private readonly AaniDbContext _context;

        public ManageCommitteesModel(AaniDbContext context)
        {
            _context = context;
        }

        public IList<EventCommitteeViewModel> EventCommittees { get; set; }

        // --- NEW: Properties for OperationYear filter ---
        [FromQuery]
        public long? OperationYearId { get; set; }
        public OperationYear CurrentOperationYear { get; set; }
        public SelectList OperationYearsSL { get; set; }

        [FromQuery(Name = "filter")]
        public string CurrentFilter { get; set; }

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

            // 2. Handle "With Committee" / "All" filter
            if (string.IsNullOrEmpty(CurrentFilter))
            {
                CurrentFilter = "with_committee";
            }

            // 3. Build the main query, now filtering by OperationYearId first
            var query = _context.Events
                .Where(e => e.OperationYearId == OperationYearId.Value) // Filter by selected year
                .Include(e => e.EventCommittes)
                .Select(e => new EventCommitteeViewModel
                {
                    EventId = e.Id,
                    EventName = e.Title,
                    EventDate = e.StartDate,
                    MemberCount = e.EventCommittes.Count()
                });

            // Apply the "With Committee" filter
            if (CurrentFilter == "with_committee")
            {
                query = query.Where(vm => vm.MemberCount > 0);
            }

            EventCommittees = await query.OrderByDescending(vm => vm.EventDate).ToListAsync();
        }
    }

    public class EventCommitteeViewModel
    {
        public long EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public int MemberCount { get; set; }
    }
}