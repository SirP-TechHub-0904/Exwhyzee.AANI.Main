using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.EventManager
{
    public class ExpenditurePageModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public ExpenditurePageModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<EventExpenditure> EventExpenditure { get; set; }
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
            EventExpenditure = await _context.EventExpenditures
                .Include(e => e.Event).Where(x=>x.EventId == id).ToListAsync();
            return Page();
        }
    }
}
