using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ExecutivePage.ActiveExecutive
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class EditModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public EditModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Executive Executive { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Executive = await _context.Executives
                .Include(e => e.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (Executive == null)
            {
                return NotFound();
            }
           ViewData["ParticipantId"] = new SelectList(_context.Set<Participant>(), "Id", "Fullname");      
            ViewData["ExecutivePositionId"] = new SelectList(_context.ExecutivePositions, "Id", "Position");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Executive).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Updated successfully";

            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["aaerror"] = "unable to update";

            }


            return RedirectToPage("./Index");
        }

        private bool ExecutiveExists(long id)
        {
            return _context.Executives.Any(e => e.Id == id);
        }
    }
}
