using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Admin.Pages.MessagePage.Category
{
    public class MsgListModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public MsgListModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<MessageTemplateContent> MessageTemplateContent { get; set; }
        public MessageTemplateCategory MessageTemplateCategory { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MessageTemplateCategory = await _context.MessageTemplateCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (MessageTemplateCategory == null)
            {
                return NotFound();
            }
            MessageTemplateContent = await _context.MessageTemplateContents.Where(x=>x.MessageTemplateCategoryId == id).ToListAsync();
            return Page();
        
        }
    }
}
