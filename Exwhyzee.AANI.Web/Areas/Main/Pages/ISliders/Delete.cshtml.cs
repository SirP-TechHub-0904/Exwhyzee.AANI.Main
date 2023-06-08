using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Exwhyzee.AANI.Domain.Models;
using Microsoft.EntityFrameworkCore;
 

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ISliders
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin,Editor")]

    public class DeleteModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DeleteModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Slider Slider { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (Slider == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Slider = await _context.Sliders.FindAsync(id);

            if (Slider != null)
            {
                _context.Sliders.Remove(Slider);
                await _context.SaveChangesAsync(); TempData["success"] = "Successful";
            }

            return RedirectToPage("./Index");
        }
    }
}
