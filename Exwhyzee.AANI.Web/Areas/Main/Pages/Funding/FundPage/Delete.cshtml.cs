using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Funding.FundPage
{
    public class DeleteModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DeleteModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Fund Fund { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Fund = await _context.Funds
                .Include(f => f.Event)
                .Include(f => f.FundCategory)
                .Include(f => f.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (Fund == null)
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

            Fund = await _context.Funds.FindAsync(id);

            if (Fund != null)
            {
                _context.Funds.Remove(Fund);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
