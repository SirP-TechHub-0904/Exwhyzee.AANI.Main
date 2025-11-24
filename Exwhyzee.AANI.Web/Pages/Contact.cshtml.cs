using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Pages
{
    public class ContactModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public ContactModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {


            ContactSettings = await _context.ContactSettings
              .FirstOrDefaultAsync();
            return Page();
        }

        [BindProperty]
        public Message Message { get; set; }

        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Phone { get; set; }

        [BindProperty]
        public string Subject { get; set; }

        [BindProperty]
        public string MessageBody { get; set; }


        public ContactSettingsModel ContactSettings { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Message.NotificationType = Domain.Enums.NotificationType.Email;
            Message.NotificationStatus = Domain.Enums.NotificationStatus.Pending;
            Message.Date = DateTime.UtcNow.AddHours(1);
            Message.Recipient = "director@aani.ng";
            Message.Title = "Contact Us";
            Message.Mail = "";
            _context.Messages.Add(Message);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
