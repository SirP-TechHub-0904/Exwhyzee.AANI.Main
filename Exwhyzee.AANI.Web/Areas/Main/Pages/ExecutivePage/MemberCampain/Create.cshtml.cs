using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ExecutivePage.MemberCampain
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
        ViewData["ExecutivePositionId"] = new SelectList(_context.ExecutivePositions, "Id", "Position");
        ViewData["ParticipantId"] = new SelectList(_context.Set<Participant>(), "Id", "Fullname");
            return Page();
        }

        [BindProperty]
        public Campain Campain { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            

            _context.Campains.Add(Campain);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
