using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.AANI.Domain.Dtos;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Exwhyzee.AANI.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    public class CreateModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly ILogger<CreateModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public CreateModel(
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

            var partaccount = _userManager.Users.Include(x=>x.SEC).AsQueryable();
            var secoutput = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname = x.Title + " " + x.Surname + " " + x.FirstName + " " + x.OtherName + "(SEC "+x.SEC.Number+"- "+x.SEC.Year+")"
            });
            ViewData["PId"] = new SelectList(secoutput, "Id", "Fullname");

            var secs = _context.SECs.AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC "+ x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");

            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }

       
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            public string? Surname { get; set; }
            [Display(Name = "First Name")]
            public string? FirstName { get; set; }
            [Display(Name = "Other Name")]
            public string? OtherName { get; set; }
            public string? Title { get; set; }
          
            public string? Sponsor { get; set; }
            public string? PhoneNumber { get; set; }
            public long SECId { get; set; }


        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            var user = new Participant
            {
                UserName = Input.Email,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                FirstName = Input.FirstName,
                Surname = Input.Surname,
                OtherName = Input.OtherName,
                Title = Input.Title,
                Sponsor = Input.Sponsor,
                SECId = Input.SECId
            };
            Guid pass = Guid.NewGuid();
            var result = await _userManager.CreateAsync(user, pass.ToString().Replace("-", ".") + "XY");

            if (result.Succeeded)
            {
                TempData["aasuccess"] = "Account created successfully";
                return RedirectToPage("./Index");
            }
            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, error.Description);
            //}
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            TempData["aaerror"] = messages;
            //
            var secs = _context.SECs.AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC " + x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");

            return Page();

        }

      
    }
}
