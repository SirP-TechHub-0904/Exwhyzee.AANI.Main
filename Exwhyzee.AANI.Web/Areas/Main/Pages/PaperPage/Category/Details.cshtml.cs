using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PaperPage.Category
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public PaperCategory PaperCategory { get; set; }
        public IList<Paper> Paper { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PaperCategory = await _context.paperCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (PaperCategory == null)
            {
                return NotFound();
            }

            Paper = await _context.Papers
               .Include(p => p.Event)
               .Include(p => p.PaperCategory)
               .Include(p => p.Participant).ToListAsync();
            return Page();
        }
    }
}
