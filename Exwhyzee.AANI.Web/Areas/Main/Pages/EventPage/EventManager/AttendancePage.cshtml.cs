using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.EventManager
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class AttendancePageModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public AttendancePageModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<EventAttendance> EventAttendance { get; set; }
        public Event Event { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Event = await _context.Events
               .FirstOrDefaultAsync(m => m.Id == id);
            if (Event == null)
            {
                return NotFound();
            }
            EventAttendance = await _context.EventAttendances
                .Include(e => e.Event)
                .Include(e => e.Participant)
                .ThenInclude(e => e.SEC)
                .Where(x => x.EventId == id).ToListAsync();
            return Page();
        }
    }
}
