using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.EventManager
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class BudgetModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public BudgetModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<EventBudget> EventBudget { get; set; }
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
            EventBudget = await _context.EventBudget
                .Include(e => e.Event).Where(x=>x.EventId == id).ToListAsync();

          
            return Page();
        }
    }
}
