using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using Exwhyzee.AANI.Domain.Models;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.IPages.List
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin,Editor")]

    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<PageSectionList> PageSectionList { get;set; }

        public async Task OnGetAsync()
        {
            PageSectionList = await _context.PageSectionLists
                .Include(p => p.PageSection)
                .Include(p => p.PageSection)

                .ToListAsync();
        }
    }
}
