using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.NewsletterPage.Templates
{
    public class EditModel : PageModel
    {
        private readonly AaniDbContext _context;
        public EditModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MessageTemplate MessageTemplate { get; set; } = new MessageTemplate();

        public List<SelectListItem> MessageTypeList { get; set; } = new();

        public void PopulateMessageTypes()
        {
            MessageTypeList = Enum.GetValues(typeof(MessageType))
                .Cast<MessageType>()
                .Select(x => new SelectListItem(x.ToString(), ((int)x).ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            PopulateMessageTypes();
            var t = await _context.MessageTemplates.FindAsync(id);
            if (t == null) return NotFound();

            MessageTemplate = t;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PopulateMessageTypes();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existing = await _context.MessageTemplates.FindAsync(MessageTemplate.Id);
            if (existing == null) return NotFound();

            existing.Name = MessageTemplate.Name;
            existing.MessageType = MessageTemplate.MessageType;
             existing.Body = MessageTemplate.Body; 
            existing.IsActive = MessageTemplate.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Template updated.";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var existing = await _context.MessageTemplates.FindAsync(MessageTemplate.Id);
            if (existing == null) return NotFound();

            _context.MessageTemplates.Remove(existing);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Template deleted.";
            return RedirectToPage("./Index");
        }
    }
}