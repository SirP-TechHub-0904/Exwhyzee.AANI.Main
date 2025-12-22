using System;
using System.Threading.Tasks;
using Exwhyzee.AANI.Web.Data;
 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account.BirthdayTemplates
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AaniDbContext _context;

        public EditModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BirthdayTemplate Input { get; set; } = new BirthdayTemplate();

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var t = await _context.BirthdayTemplates.FindAsync(id);
            if (t == null) return NotFound();

            Input = t;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var t = await _context.BirthdayTemplates.FindAsync(id);
            if (t == null) return NotFound();

 
            // update fields
            t.Name = Input.Name;
            t.SendTime = Input.SendTime;
            t.EmailSubject = Input.EmailSubject;
            t.EmailBody = Input.EmailBody;
            t.IsEnabled = Input.IsEnabled;
            t.UpdatedAt = DateTime.UtcNow;

            _context.BirthdayTemplates.Update(t);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}