using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Admin.Pages.MessagePage.Msg
{
    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CreateModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["MessageId"] = new SelectList(_context.MessageTemplateCategories, "Id", "Title");

            return Page();
        }

        [BindProperty]
        public MessageTemplateContent MessageTemplateContent { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          

            _context.MessageTemplateContents.Add(MessageTemplateContent);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
