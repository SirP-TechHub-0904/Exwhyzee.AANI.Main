using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.Committee
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public EventCommitte EventCommitte { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EventCommitte = await _context.EventCommittes
                .Include(e => e.Event)
                .Include(e => e.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (EventCommitte == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
