using System.Text.Json;
using System.Text.RegularExpressions;
using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Services.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.NewsletterPage.NotificationPage
{
    public class WhatsappModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;
        private readonly IRecipientParser _recipientParser;
        private readonly ITemplateRenderer _templateRenderer;

        public WhatsappModel(AaniDbContext context, UserManager<Participant> userManager, IRecipientParser recipientParser, ITemplateRenderer templateRenderer)
        {
            _context = context;
            _userManager = userManager;
            _recipientParser = recipientParser;
            _templateRenderer = templateRenderer;
        }

        [BindProperty]
        public long? SelectedTemplateId { get; set; }

        [BindProperty]
        public string? Body { get; set; }

        [BindProperty]
        public string? ManualRecipients { get; set; }

        [BindProperty]
        public string? RecipientSource { get; set; }

        [BindProperty]
        public long? SelectedChapterId { get; set; }

        [BindProperty]
        public long? SelectedSecId { get; set; }

        [BindProperty]
        public string? SelectedParticipantIdsJson { get; set; }

        public List<SelectListItem> TemplateList { get; set; } = new();
        public List<SelectListItem> ChapterList { get; set; } = new();
        public List<SelectListItem> SecList { get; set; } = new();

        public async Task OnGetAsync()
        {
            TemplateList = await _context.MessageTemplates
                .Where(t => t.IsActive && t.MessageType == MessageType.Whatsapp)
                .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                .ToListAsync();

            ChapterList = await _context.Chapters
                .OrderBy(x => x.State)
                .Select(x => new SelectListItem(x.State, x.Id.ToString()))
                .ToListAsync();

            var secs = await _context.SECs
               .OrderByDescending(x => x.Year)
               .Select(x => new { x.Id, x.Number, x.Year })
               .ToListAsync();

            SecList = secs
                .Select(x => new SelectListItem
                {
                    Text = $"{x.Number} - {x.Year}",
                    Value = x.Id.ToString()
                })
                .ToList();
        }

        // AJAX: return templates for messageType
        public async Task<IActionResult> OnGetTemplatesAsync(string messageType)
        {
            if (!Enum.TryParse<MessageType>(messageType, out var mt))
            {
                return new JsonResult(new object[0]);
            }

            var list = await _context.MessageTemplates
                .Where(t => t.IsActive && t.MessageType == mt)
                .Select(t => new { id = t.Id, name = t.Name })
                .ToListAsync();

            return new JsonResult(list);
        }

        // AJAX: load a template by id (return body)
        public async Task<IActionResult> OnGetTemplateLoadAsync(long id)
        {
            var t = await _context.MessageTemplates.FindAsync(id);
            if (t == null) return NotFound();
            return new JsonResult(new { body = t.Body });
        }

        // AJAX: load participants by source (preview)
        public async Task<IActionResult> OnGetParticipantsAsync(string source, long chapterId = 0, long secId = 0, bool preview = false)
        {
            IQueryable<Participant> q = _userManager.Users.Include(x => x.Chapter).Include(x => x.SEC)
                // same filters as other pages
                .Where(x => !x.Email.Contains("aani.com.ng"))
                .Where(x => !x.Email.Contains("aani.ng"))
                .Where(x => !x.Email.Contains("aani.com"))
                .Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI);

            if (source == "chapter" && chapterId > 0)
            {
                q = q.Where(x => x.ChapterId == chapterId);
            }
            else if (source == "sec" && secId > 0)
            {
                q = q.Where(x => x.SECId == secId);
            }

            if (preview)
            {
                var previewList = await q.OrderBy(x => x.Surname).Select(x => new
                {
                    id = x.Id,
                    fullname = x.Fullname,
                    phone = x.PhoneNumber,
                    sec = x.SEC != null ? x.SEC.Number + " (" + x.SEC.Year + ")" : "",
                    chapter = x.Chapter != null ? x.Chapter.State : ""
                }).ToListAsync();

                return new JsonResult(previewList);
            }

            var list = await q.OrderBy(x => x.Surname).Select(x => new
            {
                id = x.Id,
                fullname = x.Fullname,
                phone = x.PhoneNumber,
                sec = x.SEC != null ? x.SEC.Number + " (" + x.SEC.Year + ")" : "",
                chapter = x.Chapter != null ? x.Chapter.State : ""
            }).ToListAsync();

            return new JsonResult(list);
        }

        private static string NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            var digits = Regex.Replace(phone, @"\D", "");
            return digits;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (String.IsNullOrEmpty(Body))
            {
                TempData["warning"] = "content is empty";
                return RedirectToPage();
            }

            // gather recipients: parse selected participant ids from the JSON hidden field
            var recipients = new List<(string? participantId, string fullName, string? email, string? phone, long? chapterId, long? secId)>();

            List<string> selectedIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(SelectedParticipantIdsJson))
            {
                try
                {
                    selectedIds = JsonSerializer.Deserialize<List<string>>(SelectedParticipantIdsJson) ?? new List<string>();
                }
                catch
                {
                    selectedIds = new List<string>();
                }
            }

            if (selectedIds.Any())
            {
                var participants = await _userManager.Users
                    .Where(u => selectedIds.Contains(u.Id))
                    .Include(u => u.Chapter)
                    .Include(u => u.SEC)
                    .ToListAsync();

                foreach (var p in participants)
                {
                    recipients.Add((p.Id, p.Fullname, p.Email, p.PhoneNumber, p.ChapterId, p.SECId));
                }
            }

            if (!string.IsNullOrWhiteSpace(ManualRecipients))
            {
                var parseResult = _recipientParser.Parse(ManualRecipients);
                foreach (var r in parseResult.Recipients)
                {
                    recipients.Add((null, r.FullName ?? string.Empty, r.Email, r.Phone, null, null));
                }
            }

            // dedupe by normalized phone
            var dedup = recipients
                .GroupBy(r => NormalizePhone(r.phone))
                .Select(g => g.First())
                .ToList();

            var createdCount = 0;
            var skipped = new List<string>();

            foreach (var rec in dedup)
            {
                var norm = NormalizePhone(rec.phone);
                if (string.IsNullOrWhiteSpace(norm))
                {
                    // no phone -> skip
                    skipped.Add($"{rec.fullName} (no phone)");
                    continue;
                }

                // build a display name: use provided fullname if available; otherwise include a clear marker plus phone
                 

                var newBody = Body.Replace("{{fullname}}", string.IsNullOrWhiteSpace(rec.fullName) ? "Distinguish" : rec.fullName);
                 
                NotificationPath choosePath = NotificationPath.All;
                if (RecipientSource == "all")
                {
                    choosePath = NotificationPath.All;
                }
                else if (RecipientSource == "chapter")
                {
                    choosePath = NotificationPath.Chapter;
                }
                else if (RecipientSource == "sec")
                {
                    choosePath = NotificationPath.SEC;
                }

                string fullname = string.IsNullOrWhiteSpace(rec.fullName) ? "Distinguish" : rec.fullName;
                var notification = new Notification
                {
                    FullName = fullname,
                    Email = rec.email,
                    Phone = rec.phone,
                    Subject = null,
                    Content = newBody,
                    MessageType = MessageType.Whatsapp,
                    CreatedById = User?.Identity?.Name,
                    CreatedAt = DateTime.UtcNow,
                    NotificationPath = choosePath,
                    Status = NotificationStatus.Pending
                };

                _context.Notifications.Add(notification);
                createdCount++;
            }

            await _context.SaveChangesAsync();

            TempData["success"] = $"Queued {createdCount} WhatsApp notifications. Skipped: {skipped.Count}";
            if (skipped.Any())
            {
                TempData["warning"] = string.Join("; ", skipped);
            }

            return RedirectToPage();
        }
    }
}