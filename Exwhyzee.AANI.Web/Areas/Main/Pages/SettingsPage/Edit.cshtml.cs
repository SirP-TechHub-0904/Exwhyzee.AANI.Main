using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SettingsPage
{
    public class EditModel : PageModel
    {
        private readonly AaniDbContext _context;

        public EditModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ContactSettingsModel ContactSettings { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ContactSettings = await _context.ContactSettings
                .Include(x => x.Emails)
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ContactSettings == null)
            {
                return NotFound();
            }

            // Ensure lists have 3 phones, 4 emails, 4 addresses
            while (ContactSettings.PhoneNumbers.Count < 3)
                ContactSettings.PhoneNumbers.Add(new PhoneNumberInfo());
            while (ContactSettings.Emails.Count < 4)
                ContactSettings.Emails.Add(new EmailInfo());
            while (ContactSettings.Addresses.Count < 4)
                ContactSettings.Addresses.Add(new AddressInfo());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(ContactSettings).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
