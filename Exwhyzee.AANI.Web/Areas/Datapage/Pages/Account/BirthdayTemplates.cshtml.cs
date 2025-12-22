using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Services;
using Exwhyzee.AANI.Web.Services.Template;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class BirthdayTemplatesModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager; 

        public BirthdayTemplatesModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
         }

        public IList<BirthdayTemplate> Templates { get; set; } = new List<BirthdayTemplate>();

        // small preview list for the participant selector in preview modal (first 200)
        public IList<PreviewParticipant> PreviewParticipants { get; set; } = new List<PreviewParticipant>();

        public class PreviewParticipant
        {
            public string Id { get; set; } = "";
            public string Fullname { get; set; } = "";
            public string Email { get; set; } = "";
            public string Phone { get; set; } = "";
            public DateTime DOB { get; set; }
        }

        public async Task OnGetAsync()
        {
            Templates = await _context.BirthdayTemplates.OrderByDescending(t => t.UpdatedAt).ToListAsync();

            
        }

        // Create new template
        [BindProperty]
        public BirthdayTemplate InputTemplate { get; set; } = new BirthdayTemplate();

        public async Task<IActionResult> OnPostCreateAsync()
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            InputTemplate.CreatedAt = DateTime.UtcNow;
            InputTemplate.UpdatedAt = DateTime.UtcNow;
            _context.BirthdayTemplates.Add(InputTemplate);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // Edit
        public async Task<IActionResult> OnPostEditAsync(long id)
        {
            var t = await _context.BirthdayTemplates.FindAsync(id);
            if (t == null) return NotFound();

            if (!TryUpdateModelAsync(t, "InputTemplate").Result) // bind posted fields to t
            {
                return Page();
            }

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

        // Toggle enable/disable; optional cancelQueued parameter to cancel pending messages
        public async Task<IActionResult> OnPostToggleAsync(long id, bool cancelQueued = false)
        {
            var t = await _context.BirthdayTemplates.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();

            t.IsEnabled = !t.IsEnabled;
            t.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            if (cancelQueued && !t.IsEnabled)
            {
                // cancel pending messages for this template (mark Cancelled)
                var msgs = await _context.BirthdayMessages
                    .Where(m => m.Status == BirthdayMessageStatus.Pending)
                    .ToListAsync();
                foreach (var m in msgs)
                {
                    m.Status = BirthdayMessageStatus.Cancelled;
                    m.Error = "Cancelled by admin when disabling template";
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        // AJAX preview: render subject/body for a specific participant
        //public async Task<JsonResult> OnGetPreviewAsync(string participantId, long templateId)
        //{
        //    var template = await _context.BirthdayTemplates.FindAsync(templateId);
        //    if (template == null) return new JsonResult(new { success = false, error = "Template not found" });

        //    var p = await _userManager.FindByIdAsync(participantId);
        //    if (p == null) return new JsonResult(new { success = false, error = "Participant not found" });

        //    //var subject = _renderer.Render(template.EmailSubject ?? "", p, DateTime.UtcNow);
        //    //var emailBody = _renderer.Render(template.EmailBody ?? "", p, DateTime.UtcNow);
        //    //var sms = _renderer.Render(template.SmsBody ?? "", p, DateTime.UtcNow);

        //    return new JsonResult(new { success = true, subject, emailBody, sms });
        //}
    }
}