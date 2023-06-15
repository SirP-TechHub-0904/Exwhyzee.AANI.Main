using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
     [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class UpdateAccountModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;



        public UpdateAccountModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [BindProperty]
        public Participant Participant { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }

            Participant = await _userManager.FindByIdAsync(id);

            if (Participant == null)
            {
                return RedirectToPage("/Notfound", new { area = "" });
            }

            ViewData["StateId"] = new SelectList(_context.States, "StateName", "StateName");

            var secs = _context.SECs.AsQueryable();
            var output = secs.Select(x => new SecDropdownListDto
            {
                Id = x.Id,
                SecYear = "SEC " + x.Number + " (" + x.Year + ")"
            });
            ViewData["SECId"] = new SelectList(output, "Id", "SecYear");
            ViewData["MessageId"] = new SelectList(_context.MessageTemplateCategories, "Id", "Title");

            return Page();
        }
        public List<SelectListItem> LgaList { get; set; }

        public async Task<JsonResult> OnGetLGA(string id)
        {

            List<LocalGoverment> lga = new List<LocalGoverment>();

            var query = await _context.LocalGoverments
                .Include(x => x.States)
                .Where(x => x.States.StateName == id).ToListAsync();


            LgaList = query.Select(a =>
                                new SelectListItem
                                {
                                    Value = a.LGAName,
                                    Text = a.LGAName
                                }).ToList();
            return new JsonResult(LgaList);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var updateparticipant = await _userManager.FindByIdAsync(Participant.Id);
            try
            {
                updateparticipant.Surname = Participant.Surname;
                updateparticipant.FirstName = Participant.FirstName;
                updateparticipant.OtherName = Participant.OtherName;
                updateparticipant.Title = Participant.Title;
              
                updateparticipant.Sponsor = Participant.Sponsor;
                updateparticipant.GenderStatus = Participant.GenderStatus;
                updateparticipant.MaritalStatus = Participant.MaritalStatus;
                updateparticipant.ReligionStatus = Participant.ReligionStatus;
                updateparticipant.MessageTemplateCategoryId = Participant.MessageTemplateCategoryId;

                updateparticipant.SECId = Participant.SECId;
                

                await _userManager.UpdateAsync(updateparticipant);
                TempData["aasuccess"] = "Updated successfully";
            }
            catch (Exception)
            {
                ViewData["StateId"] = new SelectList(_context.States, "StateName", "StateName");

                var secs = _context.SECs.AsQueryable();
                var output = secs.Select(x => new SecDropdownListDto
                {
                    Id = x.Id,
                    SecYear = "SEC " + x.Number + " (" + x.Year + ")"
                });
                ViewData["SECId"] = new SelectList(output, "Id", "SecYear");

                TempData["aaerror"] = "Unable to update account";
                return Page();
            }

            return RedirectToPage("./Members");
        }


    }
}
