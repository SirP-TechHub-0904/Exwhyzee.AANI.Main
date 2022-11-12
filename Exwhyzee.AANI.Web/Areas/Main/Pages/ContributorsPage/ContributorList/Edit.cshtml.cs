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

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ContributorsPage.ContributorList
{
    [Microsoft.AspNetCore.Authorization.Authorize]

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
        public Contributor Contributor { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contributor = await _context.Contributors
                .Include(c => c.ContributorCategory)
                .Include(c => c.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (Contributor == null)
            {
                return NotFound();
            }
           ViewData["ContributorCategoryId"] = new SelectList(_context.ContributorCategories, "Id", "Title");

            var partaccount = _userManager.Users.AsQueryable();
            var output = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName
            });
            ViewData["ParticipantId"] = new SelectList(output, "Id", "Fullname");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
         
            _context.Attach(Contributor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Updated successfully";

            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["aaerror"] = "Unable to Add Contributors";
                ViewData["ContributorCategoryId"] = new SelectList(_context.ContributorCategories, "Id", "Title");

                var partaccount = _userManager.Users.AsQueryable();
                var output = partaccount.Select(x => new ParticipantDropdownDto
                {
                    Id = x.Id,
                    Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName
                });
                ViewData["ParticipantId"] = new SelectList(output, "Id", "Fullname");
                return Page();
            }

            return RedirectToPage("/ContributorsPage/Category/Details", new { id = Contributor.ContributorCategoryId });
        }

        private bool ContributorExists(long id)
        {
            return _context.Contributors.Any(e => e.Id == id);
        }
    }
}
