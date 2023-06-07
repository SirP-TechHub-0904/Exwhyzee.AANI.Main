using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
    public class LoadedModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly RoleManager<IdentityRole> _role;
        public LoadedModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context,
            RoleManager<IdentityRole> role)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _role = role;
        }
        public async Task<IActionResult> OnGet()
        {

            var itemlist = await _userManager.Users.Take(8000).ToListAsync();
            
            //foreach(var xitem in itemlist)
            //{
            //    var user = await _userManager.FindByIdAsync(xitem.Id);
            //    IdentityRole mni = new IdentityRole("MNI");
            //     var checkrole = await _role.FindByNameAsync("MNI");
            //     if (checkrole == null)
            //    {
            //        await _role.CreateAsync(mni);

            //    }
                
            //    await _userManager.AddToRoleAsync(user, "MNI");
 
            //}
            return Page();
        }

    }
}
