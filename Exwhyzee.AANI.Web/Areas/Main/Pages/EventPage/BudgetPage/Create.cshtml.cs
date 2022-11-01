using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.BudgetPage
{
    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CreateModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }
        public Event Event { get; set; }


        public async Task<IActionResult> OnGet(long? id)
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

            return Page();
        }

        [BindProperty]
        public EventBudget EventBudget { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
           
            _context.EventBudget.Add(EventBudget);
            await _context.SaveChangesAsync();

            return RedirectToPage("/EventPage/EventManager/Budget", new {id = EventBudget.EventId});
        }
    }
}
