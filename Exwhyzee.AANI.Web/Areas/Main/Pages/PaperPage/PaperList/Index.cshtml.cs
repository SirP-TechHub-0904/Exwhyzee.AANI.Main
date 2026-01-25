using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PaperPage.PaperList
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")] // Restricted to Admin
    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Paper> Paper { get; set; }

        public async Task OnGetAsync()
        {
            Paper = await _context.Papers
                .Include(p => p.Event)
                .Include(p => p.PaperCategory)
                .Include(p => p.Participant)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        // Handler to approve the paper
        public async Task<IActionResult> OnPostApproveAsync(long id)
        {
            var paper = await _context.Papers.FindAsync(id);

            if (paper == null)
            {
                return NotFound();
            }

            paper.AdminApproved = true;
            _context.Attach(paper).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                // You can add a TempData message here for a "Sweet Alert" style notification
                TempData["SuccessMessage"] = "Paper approved successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaperExists(paper.Id)) { return NotFound(); }
                else { throw; }
            }

            return RedirectToPage("./Index");
        }

        private bool PaperExists(long id)
        {
            return _context.Papers.Any(e => e.Id == id);
        }
    }
}