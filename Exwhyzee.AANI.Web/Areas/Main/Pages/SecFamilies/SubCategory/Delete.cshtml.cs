using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecFamilies.SubCategory
{
    public class DeleteModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DeleteModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SubCategoryFamiliesOnSEC SubCategoryFamiliesOnSEC { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubCategoryFamiliesOnSEC = await _context.SubCategoryFamiliesOnSECs
                .Include(s => s.CategoryFamiliesOnSEC).FirstOrDefaultAsync(m => m.Id == id);

            if (SubCategoryFamiliesOnSEC == null)
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

            SubCategoryFamiliesOnSEC = await _context.SubCategoryFamiliesOnSECs.FindAsync(id);

            if (SubCategoryFamiliesOnSEC != null)
            {
                _context.SubCategoryFamiliesOnSECs.Remove(SubCategoryFamiliesOnSEC);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
