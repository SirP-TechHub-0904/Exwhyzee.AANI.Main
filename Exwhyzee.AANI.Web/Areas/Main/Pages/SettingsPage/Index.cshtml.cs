using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SettingsPage
{
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;
        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ContactSettingsModel ContactSettings { get; set; }

        [BindProperty]
        public PhoneNumberInfo NewPhone { get; set; } = new PhoneNumberInfo();

        [BindProperty]
        public EmailInfo NewEmail { get; set; } = new EmailInfo();

        [BindProperty]
        public AddressInfo NewAddress { get; set; } = new AddressInfo();

        [BindProperty]
        public SocialMediaLinks SocialUpdate { get; set; } = new SocialMediaLinks();

        public async Task<IActionResult> OnGetAsync()
        {
            ContactSettings = await _context.ContactSettings
                .Include(x => x.PhoneNumbers)
                .Include(x => x.Emails)
                .Include(x => x.Addresses)
                .Include(x => x.SocialMedia)
                .FirstOrDefaultAsync();

            if (ContactSettings == null)
            {
                ContactSettings = new ContactSettingsModel();
                _context.ContactSettings.Add(ContactSettings);
                await _context.SaveChangesAsync();
            }
            return Page();
        }

        // PHONE CRUD
        public async Task<IActionResult> OnPostAddPhoneAsync()
        {
            var settings = await _context.ContactSettings.Include(x => x.PhoneNumbers).FirstOrDefaultAsync();
            if (settings != null && !string.IsNullOrWhiteSpace(NewPhone.Number))
            {
                settings.PhoneNumbers.Add(new PhoneNumberInfo { Number = NewPhone.Number });
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePhoneAsync(long id)
        {
            var phone = await _context.PhoneNumbers.FindAsync(id);
            if (phone != null)
            {
                _context.PhoneNumbers.Remove(phone);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditPhoneAsync(long id, string number)
        {
            var phone = await _context.PhoneNumbers.FindAsync(id);
            if (phone != null)
            {
                phone.Number = number;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // EMAIL CRUD
        public async Task<IActionResult> OnPostAddEmailAsync()
        {
            var settings = await _context.ContactSettings.Include(x => x.Emails).FirstOrDefaultAsync();
            if (settings != null && !string.IsNullOrWhiteSpace(NewEmail.Email))
            {
                settings.Emails.Add(new EmailInfo { Title = NewEmail.Title, Email = NewEmail.Email });
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteEmailAsync(long id)
        {
            var email = await _context.Emails.FindAsync(id);
            if (email != null)
            {
                _context.Emails.Remove(email);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditEmailAsync(long id, string title, string email)
        {
            var emailEntry = await _context.Emails.FindAsync(id);
            if (emailEntry != null)
            {
                emailEntry.Title = title;
                emailEntry.Email = email;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // ADDRESS CRUD
        public async Task<IActionResult> OnPostAddAddressAsync()
        {
            var settings = await _context.ContactSettings.Include(x => x.Addresses).FirstOrDefaultAsync();
            if (settings != null && !string.IsNullOrWhiteSpace(NewAddress.Address))
            {
                settings.Addresses.Add(new AddressInfo { Title = NewAddress.Title, Address = NewAddress.Address });
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAddressAsync(long id)
        {
            var addr = await _context.Addresses.FindAsync(id);
            if (addr != null)
            {
                _context.Addresses.Remove(addr);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAddressAsync(long id, string title, string address)
        {
            var addr = await _context.Addresses.FindAsync(id);
            if (addr != null)
            {
                addr.Title = title;
                addr.Address = address;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // SOCIAL MEDIA UPDATE
        public async Task<IActionResult> OnPostEditSocialMediaAsync()
        {
            var settings = await _context.ContactSettings.Include(x => x.SocialMedia).FirstOrDefaultAsync();
            if (settings?.SocialMedia != null)
            {
                settings.SocialMedia.Facebook = SocialUpdate.Facebook;
                settings.SocialMedia.Twitter = SocialUpdate.Twitter;
                settings.SocialMedia.Instagram = SocialUpdate.Instagram;
                settings.SocialMedia.LinkedIn = SocialUpdate.LinkedIn;
                settings.SocialMedia.Youtube = SocialUpdate.Youtube;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAboutTextAsync()
        {
            var settings = await _context.ContactSettings.FirstOrDefaultAsync();
            if (settings == null) return NotFound();

            settings.AboutText = ContactSettings.AboutText;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}