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
    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public SubCategoryFamiliesOnSEC SubCategoryFamiliesOnSEC { get; set; }
        public IList<ParticipantFamiliesOnSEC> ParticipantFamiliesOnSEC { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubCategoryFamiliesOnSEC = await _context.SubCategoryFamiliesOnSECs
                .Include(s => s.CategoryFamiliesOnSEC)
                .FirstOrDefaultAsync(m => m.Id == id);

            ParticipantFamiliesOnSEC = await _context.ParticipantFamiliesOnSECs
             .Include(p => p.SubCategoryFamiliesOnSEC)
             .Include(p => p.Participant)
             .ThenInclude(p => p.SEC)
             .Where(x=>x.SubCategoryFamiliesOnSECId == id)
             .ToListAsync();
            if (SubCategoryFamiliesOnSEC == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
