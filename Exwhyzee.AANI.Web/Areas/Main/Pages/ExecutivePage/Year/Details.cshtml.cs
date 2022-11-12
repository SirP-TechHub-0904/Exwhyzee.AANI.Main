using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ExecutivePage.Year
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public PastExecutiveYear PastExecutiveYear { get; set; }
        public IList<PastExecutiveMember> PastExecutiveMember { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PastExecutiveYear = await _context.PastExecutiveYear.FirstOrDefaultAsync(m => m.Id == id);

            if (PastExecutiveYear == null)
            {
                return NotFound();
            }
            PastExecutiveMember = await _context.PastExecutiveMembers
                .Include(p => p.Participant)
                .ThenInclude(p => p.SEC)
                .Include(p => p.PastExecutiveYear).Where(x=>x.PastExecutiveYearId == id).ToListAsync();
            return Page();
        }
    }
}
