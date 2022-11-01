using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecPage
{
    public class EditModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public EditModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SEC? SEC { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }

            SEC = await _context.SECs.FirstOrDefaultAsync(m => m.Id == id);

            if (SEC == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            

            _context.Attach(SEC).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SECExists(SEC.Id))
                {
                    return RedirectToPage("/Notfound", new { area = "" });
                }
                else
                {
                    throw;
                }
            }
            TempData["aasuccess"] = "Updated Successfully";
            return RedirectToPage("./Index");
        }

        private bool SECExists(long id)
        {
            return _context.SECs.Any(e => e.Id == id);
        }
    }
}
