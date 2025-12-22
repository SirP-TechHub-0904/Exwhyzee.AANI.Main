using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Web.Data; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account.BirthdayTemplates
{
    [Authorize(Roles = "Admin,Birthdays")]
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;

        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        public IList<BirthdayTemplate> Templates { get; set; } = new List<BirthdayTemplate>();

        public async Task OnGetAsync()
        {
            Templates = await _context.BirthdayTemplates
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .ToListAsync();
        }

        // Toggle enable/disable
        public async Task<IActionResult> OnPostToggleAsync(long id)
        {
            var t = await _context.BirthdayTemplates.FindAsync(id);
            if (t == null) return NotFound();

            t.IsEnabled = !t.IsEnabled;
            t.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // Delete
        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var t = await _context.BirthdayTemplates.FindAsync(id);
            if (t == null) return NotFound();

            _context.BirthdayTemplates.Remove(t);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}