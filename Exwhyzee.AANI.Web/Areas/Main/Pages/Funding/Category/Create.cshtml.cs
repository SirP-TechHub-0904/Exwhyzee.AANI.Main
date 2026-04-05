using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Funding.Category
{
    [Authorize(Roles = "Admin,ChapterAdmin")]
    public class CreateModel : PageModel
    {
      
            private readonly AaniDbContext _context;
            private readonly UserManager<Participant> _userManager;

            public CreateModel(AaniDbContext context, UserManager<Participant> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            // Is the current user a national admin or chapter admin?
            public bool IsNationalAdmin { get; set; }
            public long? CurrentUserChapterId { get; set; }

            public async Task<IActionResult> OnGetAsync()
            {
                var user = await _userManager.GetUserAsync(User);
                IsNationalAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                CurrentUserChapterId = user?.ChapterId;

                await PopulateDropdowns();
                return Page();
            }

            [BindProperty]
            public FundCategory FundCategory { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            IsNationalAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // If chapter admin, force-assign their chapter & mark as chapter-level
            if (!IsNationalAdmin)
            {
                FundCategory.ChapterId = user?.ChapterId;
                FundCategory.IsNationalLevel = false;
            }

            // If national admin chose chapter-specific, ChapterId is required
            if (IsNationalAdmin && !FundCategory.IsNationalLevel && FundCategory.ChapterId == null)
            {
                ModelState.AddModelError("FundCategory.ChapterId", "Please select a chapter.");
            }

            // Amount only required if not flexible
            if (!FundCategory.IsFlexible && (FundCategory.Amount == null || FundCategory.Amount <= 0))
            {
                ModelState.AddModelError("FundCategory.Amount", "Amount is required for fixed payments.");
            }

            // Clear amount if flexible — don't save a stale value
            if (FundCategory.IsFlexible)
            {
                FundCategory.Amount = null;
                ModelState.Remove("FundCategory.Amount");
            }

            // Shares must total 100 if percentage split
            if (FundCategory.SplitType == SplitType.Percentage)
            {
                if (FundCategory.NationalShare + FundCategory.ChapterShare != 100)
                {
                    ModelState.AddModelError("", "National Share and Chapter Share must total 100%.");
                }
            }

            // If NationalShare is 0, chapter gets everything — set ChapterShare to 100
            if (FundCategory.NationalShare == 0)
            {
                FundCategory.ChapterShare = 100;
                ModelState.Remove("FundCategory.ChapterShare");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return Page();
            }

            _context.FundCategories.Add(FundCategory);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        private async Task PopulateDropdowns()
            {
                ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title");
                ViewData["SplitType"] = new SelectList(Enum.GetValues(typeof(SplitType))
                    .Cast<SplitType>()
                    .Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text");
            }
        }
}
