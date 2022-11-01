using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ContributorsPage.Category
{
    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IQueryable<Contributor> Contributor { get; set; }
        public ContributorCategory ContributorCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contributor = _context.Contributors
                .Include(x=>x.Participant)
                .ThenInclude(x=>x.SEC)
                .Where(m => m.ContributorCategoryId == id).AsQueryable();

            ContributorCategory = await _context.ContributorCategories.FirstOrDefaultAsync(m => m.Id == id);

            return Page();
        }
    }
}
