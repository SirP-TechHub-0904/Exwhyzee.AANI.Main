using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    //[Authorize(Roles = "mSuperAdmin")]

    public class PermissionModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Participant> _signInManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;



        public PermissionModel(
            UserManager<Participant> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<Participant> signInManager,
            Data.AaniDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }


        public IList<string> Roles { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<string> RemainingUserRoles { get; set; }

        [BindProperty]
        public Participant Participant { get; set; }
      

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Participant = await _userManager.FindByIdAsync(id);
 Roles = await _roleManager.Roles.Where(x => x.Name != "mSuperAdmin").Select(x => x.Name).ToListAsync();


            UserRoles = await _userManager.GetRolesAsync(Participant);
           

                var RemainingRoles = Roles.Except(UserRoles);
            RemainingUserRoles = RemainingRoles.ToList();

            if (Participant == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
             var role = await _roleManager.FindByIdAsync(id);
            //var user = await _userManager.FindByIdAsync(UserId);
            var checkuserroles = await _userManager.IsInRoleAsync(Participant, role.Name);
            if (checkuserroles == true)
            {
                try
                {
                    await _userManager.RemoveFromRoleAsync(Participant, role.Name);
                    TempData["aasuccess"] = "permission update successfully";
                }
                catch (Exception d) {

                    TempData["aaerror"] = "Unable to update permission";
                }
            }
            else
            {
                try
                {
                    await _userManager.AddToRoleAsync(Participant, role.Name);
                    TempData["aasuccess"] = "permission update successfully";
                }
                catch (Exception d) {

                    TempData["aaerror"] = "Unable to update permission";
                }
            }


            return RedirectToPage("./Permission", new { uid = Participant.Id, fullname = Participant.Fullname });
        }
           
    }
}
