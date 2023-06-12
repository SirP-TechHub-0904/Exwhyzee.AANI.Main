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

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecFamilies.ParticipantMember
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
        public ParticipantFamiliesOnSEC ParticipantFamiliesOnSEC { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ParticipantFamiliesOnSEC = await _context.ParticipantFamiliesOnSECs
                .Include(p => p.SubCategoryFamiliesOnSEC).FirstOrDefaultAsync(m => m.Id == id);

            if (ParticipantFamiliesOnSEC == null)
            {
                return NotFound();
            }
           ViewData["SubCategoryFamiliesOnSECId"] = new SelectList(_context.SubCategoryFamiliesOnSECs, "Id", "Title");
            var partaccount = _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).AsQueryable();
            var secoutput = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname =  x.Surname + " " + x.FirstName + " " + x.OtherName + "(SEC " + x.SEC.Number + "- " + x.SEC.Year + ")"
            });
            ViewData["PId"] = new SelectList(secoutput, "Id", "Fullname");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
          

            _context.Attach(ParticipantFamiliesOnSEC).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); TempData["aasuccess"] = "Updated successfully";

            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["aaerror"] = "unable to update";

            }

            return RedirectToPage("/SecFamilies/SubCategory/Details", new { id = ParticipantFamiliesOnSEC.SubCategoryFamiliesOnSECId });
        }

        private bool ParticipantFamiliesOnSECExists(long id)
        {
            return _context.ParticipantFamiliesOnSECs.Any(e => e.Id == id);
        }
    }
}
