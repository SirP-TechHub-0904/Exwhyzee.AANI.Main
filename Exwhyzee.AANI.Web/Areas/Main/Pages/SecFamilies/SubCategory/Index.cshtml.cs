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
    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<SubCategoryFamiliesOnSEC> SubCategoryFamiliesOnSEC { get;set; }

        public async Task OnGetAsync()
        {
            SubCategoryFamiliesOnSEC = await _context.SubCategoryFamiliesOnSECs
                .Include(s => s.CategoryFamiliesOnSEC).ToListAsync();
        }
    }
}
