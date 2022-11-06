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

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Funding.Category
{
    public class EditModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public EditModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FundCategory FundCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FundCategory = await _context.FundCategories
                .Include(f => f.Event).FirstOrDefaultAsync(m => m.Id == id);

            if (FundCategory == null)
            {
                return NotFound();
            }
           ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id");
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

            _context.Attach(FundCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Updated successfully";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["aaerror"] = "Unable to update";
            }

            return RedirectToPage("./Index");
        }

        private bool FundCategoryExists(long id)
        {
            return _context.FundCategories.Any(e => e.Id == id);
        }
    }
}
