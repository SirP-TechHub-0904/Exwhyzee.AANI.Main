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

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecFamilies.SubCategory
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
           ViewData["CategoryFamiliesOnSECId"] = new SelectList(_context.CategoryFamiliesOnSECs, "Id", "Title");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
         
            _context.Attach(SubCategoryFamiliesOnSEC).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); TempData["aasuccess"] = "Updated successfully";

            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["aaerror"] = "unable to update";

            }

            return RedirectToPage("/SecFamilies/Category/Details", new { id = SubCategoryFamiliesOnSEC.CategoryFamiliesOnSECId });
        }

        private bool SubCategoryFamiliesOnSECExists(long id)
        {
            return _context.SubCategoryFamiliesOnSECs.Any(e => e.Id == id);
        }
    }
}
