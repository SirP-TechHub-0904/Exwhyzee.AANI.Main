using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PaperPage.PaperList
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
        public Paper Paper { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Paper = await _context.Papers
                .Include(p => p.Event)
                .Include(p => p.PaperCategory)
                .Include(p => p.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (Paper == null)
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

            Paper = await _context.Papers.FindAsync(id);

            if (Paper != null)
            {
                _context.Papers.Remove(Paper);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
