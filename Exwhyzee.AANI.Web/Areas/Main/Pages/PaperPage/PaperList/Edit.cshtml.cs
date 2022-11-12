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

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PaperPage.PaperList
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
        public Paper Paper { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Paper = await _context.Papers
                .Include(p => p.Event)
                .Include(p => p.PaperCategory)
                .Include(p => p.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (Paper == null)
            {
                return NotFound();
            }
           ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title");
           ViewData["PaperCategoryId"] = new SelectList(_context.paperCategories, "Id", "Title");
            var partaccount = _userManager.Users.Include(x => x.SEC).AsQueryable();
            var secoutput = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName + "(SEC " + x.SEC.Number + "- " + x.SEC.Year + ")"
            });
            ViewData["ParticipantId"] = new SelectList(secoutput, "Id", "Fullname"); return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
           

            _context.Attach(Paper).State = EntityState.Modified;

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

            return RedirectToPage("/PaperPage/Category/Details", new {id = Paper.PaperCategoryId});
        }

        private bool PaperExists(long id)
        {
            return _context.Papers.Any(e => e.Id == id);
        }
    }
}
