using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Authorize(Roles = "Admin,MNI")]
    public class CampainPageModel : PageModel
    {
        private readonly AaniDbContext _context;

        public CampainPageModel(AaniDbContext context)
        {
            _context = context;
        }

        public IList<ExecutivePosition> ExecutivePosition { get; set; }
        public OperationYear CurrentCampainYear { get; set; }
        public long CurrentYearId { get; set; }

        public async Task<IActionResult> OnGetAsync(long yearId)
        {
            CurrentYearId = yearId;
            CurrentCampainYear = await _context.OperationYears.FindAsync(yearId);

            if (CurrentCampainYear == null)
            {
                return NotFound(); // If the year doesn't exist, return a 404
            }
            
            // The query now filters the included Campains by the yearId
            ExecutivePosition = await _context.ExecutivePositions
                .Include(ep => ep.Campains.Where(c => c.OperationYearId == yearId))
                .ToListAsync();

            return Page();
        }
    }
}