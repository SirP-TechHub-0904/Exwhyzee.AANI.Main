using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PrincipalOfficePage.Office
{
    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public PrincipalOfficerCategory PrincipalOfficerCategory { get; set; }
        public IList<PrincipalOfficer> PrincipalOfficer { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PrincipalOfficerCategory = await _context.PrincipalOfficerCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (PrincipalOfficerCategory == null)
            {
                return NotFound();
            }
            PrincipalOfficer = await _context.PrincipalOfficers
                .Include(p => p.Participant)
                .Include(p => p.PrincipalOfficerCategory).ToListAsync();
            return Page();
        }
    }
}
