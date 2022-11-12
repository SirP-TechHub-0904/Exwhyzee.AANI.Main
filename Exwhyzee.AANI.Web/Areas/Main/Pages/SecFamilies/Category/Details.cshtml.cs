using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecFamilies.Category
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<SubCategoryFamiliesOnSEC> SubCategoryFamiliesOnSEC { get; set; }
        public CategoryFamiliesOnSEC CategoryFamiliesOnSEC { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubCategoryFamiliesOnSEC = await _context.SubCategoryFamiliesOnSECs
                           .Include(s => s.CategoryFamiliesOnSEC).Where(x=>x.CategoryFamiliesOnSECId == id).ToListAsync();
            CategoryFamiliesOnSEC = await _context.CategoryFamiliesOnSECs.FirstOrDefaultAsync(m => m.Id == id);

            return Page();
        }
    }
}
