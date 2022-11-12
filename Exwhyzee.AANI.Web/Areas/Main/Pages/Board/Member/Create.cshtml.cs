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
using System.Data.Entity;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Board.Member
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
        ViewData["BoardOfGovornorCategoryId"] = new SelectList(_context.BoardOfGovornorCategories, "Id", "Title");
            var partaccount = _userManager.Users.Include(x => x.SEC).AsQueryable();
            var secoutput = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName + "(SEC " + x.SEC.Number + "- " + x.SEC.Year + ")"
            });
            ViewData["ParticipantId"] = new SelectList(secoutput, "Id", "Fullname");
            return Page();
        }

        [BindProperty]
        public BoardOfGovornorMember BoardOfGovornorMember { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            

            _context.BoardOfGovornorMembers.Add(BoardOfGovornorMember);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "Updated successfully";

            return RedirectToPage("/Board/Year/Details", new {id = BoardOfGovornorMember.BoardOfGovornorCategoryId});
        }
    }
}
