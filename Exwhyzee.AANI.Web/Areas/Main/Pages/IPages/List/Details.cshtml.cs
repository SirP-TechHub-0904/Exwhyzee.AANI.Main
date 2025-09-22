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

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public PageSectionList PageSectionList { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PageSectionList = await _context.PageSectionLists
                .Include(p => p.PageSection).FirstOrDefaultAsync(m => m.Id == id);

            if (PageSectionList == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
