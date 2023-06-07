using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AANI.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Dtos;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
    [AllowAnonymous]
    public class VerifyModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public VerifyModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult OnGet()
        {
           

            var secs = _context.SECs.OrderByDescending(x=>x.Year).AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC " + x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");
            return Page();
        }


        [BindProperty]
        public InputModel Input { get; set; }


        public class InputModel
        {
            [Required]
            public string? SEC { get; set; }

            [Required]
            public string? PID { get; set; }

        }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var member = await _userManager.FindByIdAsync(Input.PID);
            if(User != null)
            {

                return RedirectToPage("./CompleteProcess", new {id =  Input.PID});

            }

            TempData["error"] = "something happened. unable to continue. try again";
            return RedirectToPage("./Index");
        }

        public List<SelectListItem> LgaList { get; set; }

        public async Task<JsonResult> OnGetLGA(long id)
        {
            var partaccount =await _userManager.Users.Include(x => x.SEC).Where(x=>x.SECId == id).OrderBy(x=>x.Surname).ToListAsync();

            var countf = partaccount.Count();
            LgaList = partaccount.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName
            }).ToList();
            
            return new JsonResult(LgaList);
        }

    }
}
