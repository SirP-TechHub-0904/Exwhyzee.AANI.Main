using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.NewsletterPage.Templates
{
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;
        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        public IList<MessageTemplate> Templates { get; set; } = new List<MessageTemplate>();

        public async Task OnGetAsync()
        {
            Templates = await _context.MessageTemplates
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .ToListAsync();
        }
    }
}