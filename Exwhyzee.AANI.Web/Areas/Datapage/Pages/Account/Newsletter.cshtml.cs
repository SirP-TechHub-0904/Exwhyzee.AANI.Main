using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    public class NewsletterModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public NewsletterModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Notification> EmailHistory { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            // Fetch only Email type notifications sent to this specific user
            EmailHistory = await _context.Notifications
                .Where(n => n.Email == user.Email && n.MessageType == MessageType.Email)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Page();
        }

        // Handler for AJAX modal content
        public async Task<JsonResult> OnGetDetailsAsync(long id)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null) return new JsonResult(new { error = "Not found" });

            return new JsonResult(new
            {
                subject = notification.Subject,
                content = notification.Content,
                date = notification.SentAt?.ToString("f") ?? notification.CreatedAt.ToString("f")
            });
        }
    }
}