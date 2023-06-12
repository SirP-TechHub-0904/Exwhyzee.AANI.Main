using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "MNI")]

    public class ContestantsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public ContestantsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Campain> Campain { get;set; }
        public ExecutivePosition ExecutivePosition { get;set; }

        public async Task OnGetAsync(long id)
        {
            Campain = await _context.Campains
                .Include(c => c.ExecutivePosition)
                .Include(c => c.Participant).ThenInclude(x=>x.SEC).Where(x=>x.ExecutivePositionId == id).ToListAsync();

            ExecutivePosition = await _context.ExecutivePositions.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
