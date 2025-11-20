using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Migrations;

namespace Exwhyzee.AANI.Web.Pages
{
    public class EventInfoModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public EventInfoModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public Event Event { get; set; }
        public ContactSettingsModel ContactSettings { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = await _context.Events
                .Include(x=>x.EventComments)
                .ThenInclude(x=>x.Participant)
                .Include(x=>x.EventCommittes)
                .Include(x=>x.EventAttendances)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Event == null)
            {
                return NotFound();
            }
            ContactSettings = await _context.ContactSettings
              .FirstOrDefaultAsync();
            return Page();
        }

        [BindProperty]
        public EventComment EventComment { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            
            _context.EventComments.Add(EventComment);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Updated successfully";

            return RedirectToPage("/EventInfo", new {id = EventComment.EventId});
        }
    }
}
