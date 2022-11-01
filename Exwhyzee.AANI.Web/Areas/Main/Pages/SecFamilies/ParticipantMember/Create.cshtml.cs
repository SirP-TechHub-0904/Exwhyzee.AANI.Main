using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Domain.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecFamilies.ParticipantMember
{
    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public CreateModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
        ViewData["SubCategoryFamiliesOnSECId"] = new SelectList(_context.SubCategoryFamiliesOnSECs, "Id", "Title");

            var partaccount = _userManager.Users.Include(x => x.SEC).AsQueryable();
            var secoutput = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName + "(SEC " + x.SEC.Number + "- " + x.SEC.Year + ")"
            });
            ViewData["PId"] = new SelectList(secoutput, "Id", "Fullname");

            return Page();
        }

        [BindProperty]
        public ParticipantFamiliesOnSEC ParticipantFamiliesOnSEC { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          

            _context.ParticipantFamiliesOnSECs.Add(ParticipantFamiliesOnSEC);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Updated successfully";

            return RedirectToPage("/SecFamilies/SubCategory/Details", new {id = ParticipantFamiliesOnSEC.SubCategoryFamiliesOnSECId});
        }
    }
}
