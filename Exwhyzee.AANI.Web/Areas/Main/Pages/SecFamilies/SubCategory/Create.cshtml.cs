using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecFamilies.SubCategory
{
    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CreateModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryFamiliesOnSECId"] = new SelectList(_context.CategoryFamiliesOnSECs, "Id", "Title");
            return Page();
        }

        [BindProperty]
        public SubCategoryFamiliesOnSEC SubCategoryFamiliesOnSEC { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
         

            _context.SubCategoryFamiliesOnSECs.Add(SubCategoryFamiliesOnSEC);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Updated successfully";

            return RedirectToPage("/SecFamilies/Category/Details", new { id = SubCategoryFamiliesOnSEC.CategoryFamiliesOnSECId });
        }
    }
}
