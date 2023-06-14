using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class CampainPageModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CampainPageModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<ExecutivePosition> ExecutivePosition { get;set; }

        public async Task OnGetAsync()
        {
            ExecutivePosition = await _context.ExecutivePositions.Include(x=>x.Campains).ToListAsync();
        }
    }
}
