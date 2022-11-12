using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ExecutivePage.ActiveExecutive
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Executive> Executive { get;set; }

        public async Task OnGetAsync()
        {
            Executive = await _context.Executives
                .Include(e => e.Participant).ThenInclude(x=>x.SEC).ToListAsync();
        }
    }
}
