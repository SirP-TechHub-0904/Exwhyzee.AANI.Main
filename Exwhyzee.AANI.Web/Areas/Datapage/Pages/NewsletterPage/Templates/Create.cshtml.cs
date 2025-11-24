using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.NewsletterPage.Templates
{
    public class CreateModel : PageModel
    {
        private readonly AaniDbContext _context;
        public CreateModel(AaniDbContext context)
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

        public void OnGet()
        {
            PopulateMessageTypes();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PopulateMessageTypes();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            MessageTemplate.CreatedAt = DateTime.UtcNow;
            MessageTemplate.UpdatedAt = DateTime.UtcNow;
            _context.MessageTemplates.Add(MessageTemplate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Template created.";
            return RedirectToPage("./Index");
        }
    }
}