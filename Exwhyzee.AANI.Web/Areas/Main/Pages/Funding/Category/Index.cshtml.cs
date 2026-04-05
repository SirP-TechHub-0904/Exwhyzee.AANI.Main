using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Funding.Category
{
    [Authorize(Roles = "Admin,ChapterAdmin")]
    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<FundCategory> FundCategory { get;set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isChapterAdmin = await _userManager.IsInRoleAsync(user, "ChapterAdmin");
            if (isAdmin)
            {
                // National admin sees everything
                FundCategory = await _context.FundCategories
                    .Include(f => f.Event)
                    .Include(f => f.Chapter)
                    .ToListAsync();
            }
            else if (isChapterAdmin)
            {
                // Chapter admin/member sees national-level + their chapter's own
                var chapterId = user?.ChapterId;
                FundCategory = await _context.FundCategories
                    .Include(f => f.Event)
                    .Include(f => f.Chapter)
                    .Where(f => f.IsNationalLevel || f.ChapterId == chapterId)
                    .ToListAsync();
            }
        }
    }
}
