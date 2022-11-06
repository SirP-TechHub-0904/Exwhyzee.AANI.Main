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
using Exwhyzee.AANI.Domain.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Funding.FundPage
{
    public class EditModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public EditModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Fund Fund { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Fund = await _context.Funds
                .Include(f => f.Event)
                .Include(f => f.FundCategory)
                .Include(f => f.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (Fund == null)
            {
                return NotFound();
            }
           ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id");
           ViewData["FundCategoryId"] = new SelectList(_context.FundCategories, "Id", "Id");
            var partaccount = _userManager.Users.AsQueryable();
            var output = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName
            });
            ViewData["ParticipantId"] = new SelectList(output, "Id", "Fullname"); return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
           


            _context.Attach(Fund).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Updated successfully";

            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["aaerror"] = "Unable to update Paper";

                var partaccount = _userManager.Users.AsQueryable();
                var output = partaccount.Select(x => new ParticipantDropdownDto
                {
                    Id = x.Id,
                    Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName
                });
                ViewData["ParticipantId"] = new SelectList(output, "Id", "Fullname");
                return Page();
            }


            return RedirectToPage("/Funding/Category/Details", new { id = Fund.FundCategoryId });
        }

        private bool FundExists(long id)
        {
            return _context.Funds.Any(e => e.Id == id);
        }
    }
}
