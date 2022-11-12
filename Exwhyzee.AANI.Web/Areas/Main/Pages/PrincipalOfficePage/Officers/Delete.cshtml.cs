using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PrincipalOfficePage.Officers
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class DeleteModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DeleteModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PrincipalOfficer PrincipalOfficer { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PrincipalOfficer = await _context.PrincipalOfficers
                .Include(p => p.Participant)
                .Include(p => p.PrincipalOfficerCategory).FirstOrDefaultAsync(m => m.Id == id);

            if (PrincipalOfficer == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PrincipalOfficer = await _context.PrincipalOfficers.FindAsync(id);

            if (PrincipalOfficer != null)
            {
                _context.PrincipalOfficers.Remove(PrincipalOfficer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
