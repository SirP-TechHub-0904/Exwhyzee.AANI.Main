using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.NewsletterPage.NotificationPage
{
    public class HistoryModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly ILogger<HistoryModel> _logger;

        public HistoryModel(AaniDbContext context, ILogger<HistoryModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string SelectedMessageType { get; set; } = "all";

        public List<SelectListItem> MessageTypeList { get; set; } = new();
        public void OnGet(string? messageType)
        {
            if (!string.IsNullOrWhiteSpace(messageType))
                SelectedMessageType = messageType;

            MessageTypeList = new List<SelectListItem>
    {
        new SelectListItem("All", "all") { Selected = SelectedMessageType == "all" },
        new SelectListItem("Email", "Email") { Selected = SelectedMessageType == "Email" },
        new SelectListItem("SMS", "Sms") { Selected = SelectedMessageType == "Sms" },
        new SelectListItem("WhatsApp", "Whatsapp") { Selected = SelectedMessageType == "Whatsapp" }
    };
        }
        private class NotificationListItem
        {
            public long Id { get; set; }
            public string CreatedAt { get; set; } = "";
            public string? SentAt { get; set; }
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string MessageType { get; set; } = "";
            public string Status { get; set; } = "";
            public int Retries { get; set; }
            public string Chapter { get; set; } = "";
            public string Sec { get; set; } = "";
        }
        // GET: /History?handler=List
        public async Task<IActionResult> OnGetListAsync(string messageType = "all", string sort = "createdDesc", int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            // pageSize == 0 means "All"
            if (pageSize < 0) pageSize = 20;

            IQueryable<Notification> q = _context.Notifications.AsNoTracking();

            // filter by message type
            if (!string.IsNullOrWhiteSpace(messageType) && messageType.ToLowerInvariant() != "all")
            {
                if (Enum.TryParse<MessageType>(messageType, ignoreCase: true, out var mt))
                {
                    q = q.Where(n => n.MessageType == mt);
                }
            }
             
            // apply ordering
            q = sort switch
            {
                "sentDesc" => q.OrderByDescending(n => n.SentAt),
                "sentAsc" => q.OrderBy(n => n.SentAt),
                "createdAsc" => q.OrderBy(n => n.CreatedAt),
                "createdDesc" => q.OrderByDescending(n => n.CreatedAt),
                "nameAsc" => q.OrderBy(n => n.FullName),
                "nameDesc" => q.OrderByDescending(n => n.FullName), 
                _ => q.OrderByDescending(n => n.CreatedAt),
            };

            // total count for paging (full count)
            var total = await q.CountAsync();
             
            List<NotificationListItem> items;
            if (pageSize == 0) // "All"
            {
                items = await q
                    .Select(n => new NotificationListItem {
                        Id = n.Id,
                        CreatedAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        SentAt = n.SentAt.HasValue ? n.SentAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                        FullName = n.FullName,
                        Email = n.Email,
                        Phone = n.Phone,
                        MessageType = n.MessageType.ToString(),
                        Status = n.Status.ToString(),
                        Retries = n.Retries,
                        
                    })
                    .ToListAsync();
            }
            else
            {
                items = await q
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new NotificationListItem {
                        Id = n.Id,
                        CreatedAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        SentAt = n.SentAt.HasValue ? n.SentAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                        FullName = n.FullName,
                        Email = n.Email,
                        Phone = n.Phone,
                        MessageType = n.MessageType.ToString(),
                        Status = n.Status.ToString(),
                        Retries = n.Retries,
                         
                    })
                    .ToListAsync();
            }
         
            return new JsonResult(new { items = items, total = total });
        }

        // GET: /History?handler=Details&id=#
        public async Task<IActionResult> OnGetDetailsAsync(long id)
        {
            var n = await _context.Notifications
                .AsNoTracking() 
                .FirstOrDefaultAsync(x => x.Id == id);

            if (n == null) return NotFound();

            var result = new
            {
                id = n.Id,
                fullName = n.FullName,
                email = n.Email,
                phone = n.Phone,
                messageType = n.MessageType.ToString(),
                status = n.Status.ToString(),
                createdAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                sentAt = n.SentAt.HasValue ? n.SentAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                retries = n.Retries,
                subject = n.Subject,
                content = n.Content,
                responseMessage = n.ResponseMessage, 
            };

            return new JsonResult(result);
        }

        // POST: /History?handler=Resend
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostResendAsync(long id)
        {
            var n = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if (n == null) return new JsonResult(new { success = false, error = "Not found" });

            // only allow changing to pending if not already pending; always reset retries and clear response/sent
            n.Status = NotificationStatus.Pending;
            n.Retries = 0;
            n.ResponseMessage = null;
            n.SentAt = null;

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}