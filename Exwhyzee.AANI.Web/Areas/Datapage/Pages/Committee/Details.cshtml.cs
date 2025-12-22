using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Services.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Committee
{
    public class DetailsModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;
        private readonly ITemplateRenderer _templateRenderer;

        public DetailsModel(AaniDbContext context, UserManager<Participant> userManager, ITemplateRenderer templateRenderer)
        {
            _context = context;
            _userManager = userManager;
            _templateRenderer = templateRenderer;
        }

        [BindProperty]
        public Exwhyzee.AANI.Domain.Models.Committee Committee { get; set; } = new Exwhyzee.AANI.Domain.Models.Committee();

        // For edit/delete handlers
        [BindProperty]
        public Exwhyzee.AANI.Domain.Models.Committee EditCommittee { get; set; } = new Exwhyzee.AANI.Domain.Models.Committee();

        // Notify bindings
        [BindProperty]
        public long? SelectedTemplateId { get; set; }

        [BindProperty]
        public string? NotifySubject { get; set; }

        [BindProperty]
        public string? NotifyBody { get; set; }

        // UI lists
        public List<SelectListItem> TemplateList { get; set; } = new();

        // Default fallback subject/body shown when no template selected
        public string DefaultNotifySubject { get; set; } = "Notice from {{committeeName}}";
        public string DefaultNotifyBody { get; set; } = "Hello {{fullname}},\n\nThis is a message from the {{committeeName}}. Please check your dashboard for details.\n\nRegards,\nAANI Secretariat";

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var c = await _context.Committees
                .Include(x => x.Members)
                .ThenInclude(m => m.Participant)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return NotFound();

            Committee = c;
             

            // Keep defaults in NotifySubject/NotifyBody only if no selected template
            NotifySubject = DefaultNotifySubject;
            NotifyBody = DefaultNotifyBody;

            return Page();
        }

       

        // POST: notify committee members with rendered messages (queues Notification rows)
        public async Task<IActionResult> OnPostNotifyAsync()
        {
            // reload committee and members
            var committee = await _context.Committees
                .Include(x => x.Members)
                .ThenInclude(m => m.Participant)
                .FirstOrDefaultAsync(x => x.Id == Committee.Id);

            if (committee == null) return NotFound();

             

            var created = 0;
            var skipped = new List<string>();

            foreach (var m in committee.Members)
            {
                var p = m.Participant;
                if (p == null) continue;

                if (string.IsNullOrWhiteSpace(p.Email))
                {
                    skipped.Add($"{p.Fullname} (no email)");
                    continue;
                }

                NotifySubject = DefaultNotifySubject;
                NotifyBody = DefaultNotifyBody;


                string finalSubject = NotifySubject.Replace("{{committeeName}}", committee.Name);
                string finalBody = NotifyBody.Replace("{{committeeName}}", committee.Name)
                    .Replace("{{fullname}}", p.Fullname);

                
                var notification = new Notification
                { 
                    FullName = p.Fullname,
                    Email = p.Email,
                    Phone = p.PhoneNumber,
                    Subject = finalSubject,
                    Content = finalBody,
                    MessageType = MessageType.Email, 
                    CreatedById = User?.Identity?.Name,
                    CreatedAt = DateTime.UtcNow, 
                };

                _context.Notifications.Add(notification);
                created++;
            }

            await _context.SaveChangesAsync();

            TempData["success"] = $"Sent {created} emails. Skipped: {skipped.Count}";
            if (skipped.Any()) TempData["warning"] = string.Join("; ", skipped);

            return RedirectToPage(new { id = committee.Id });
        }

      
        // Delete handler (existing)
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var existing = await _context.Committees.FindAsync(Committee.Id);
            if (existing == null) return NotFound();

            _context.Committees.Remove(existing);
            await _context.SaveChangesAsync();

            TempData["success"] = "Committee deleted.";
            return RedirectToPage("./Index");
        }
    }
}