using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Funding.Category
{
    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public FundCategory FundCategory { get; set; }
        public IList<Fund> Fund { get; set; }

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
            Fund = await _context.Funds
              .Include(f => f.Event)
              .Include(f => f.FundCategory)
              .Include(f => f.Participant).ToListAsync();
            return Page();
        }
    }
}
