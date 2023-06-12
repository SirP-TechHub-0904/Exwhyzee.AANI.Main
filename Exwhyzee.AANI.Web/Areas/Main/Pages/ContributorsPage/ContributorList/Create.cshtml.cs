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

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ContributorsPage.ContributorList
{
    [Microsoft.AspNetCore.Authorization.Authorize]

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
        ViewData["ContributorCategoryId"] = new SelectList(_context.ContributorCategories, "Id", "Title");

            var partaccount = _userManager.Users.Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).AsQueryable();
            var output = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname =  x.Surname + " " + x.FirstName + " " + x.OtherName
            });
            ViewData["ParticipantId"] = new SelectList(output, "Id", "Fullname"); return Page();
        }

        [BindProperty]
        public Contributor Contributor { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {

                _context.Contributors.Add(Contributor);
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "Updated successfully";
            }
            catch (Exception)
            {
                TempData["aaerror"] = "Unable to Add Contributors";

            }
            return RedirectToPage("/ContributorsPage/Category/Details", new {id = Contributor.ContributorCategoryId});
        }
    }
}
