using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Authorize(Roles = "Admin,MNI")]
    public class CampaignYearsModel : PageModel
    {
        private readonly AaniDbContext _context;

        public CampaignYearsModel(AaniDbContext context)
        {
            _context = context;
        }
         
        // The page now lists OperationYear entities.
        public IList<OperationYear> OperationYears { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch all OperationYears, ordered by the most recent start date.
            OperationYears = await _context.OperationYears
                .OrderByDescending(oy => oy.StartDate)
                .ToListAsync();
        }
    }
}