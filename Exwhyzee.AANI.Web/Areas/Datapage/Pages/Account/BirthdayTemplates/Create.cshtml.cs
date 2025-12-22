using System;
using System.Threading.Tasks;
using Exwhyzee.AANI.Web.Data;
 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account.BirthdayTemplates
{
    [Authorize(Roles = "Admin,Birthdays")]
    public class CreateModel : PageModel
    {
        private readonly AaniDbContext _context;

        public CreateModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BirthdayTemplate Input { get; set; } = new BirthdayTemplate();

        public void OnGet()
        {
            // default send time if needed
            Input.SendTime = TimeSpan.FromHours(9);
            Input.IsEnabled = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
 
            Input.CreatedAt = DateTime.UtcNow;
            Input.UpdatedAt = DateTime.UtcNow;
            _context.BirthdayTemplates.Add(Input);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}