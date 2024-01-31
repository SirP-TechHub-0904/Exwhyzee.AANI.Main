using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
 using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages
{
    [AllowAnonymous]

    public class IdentifyModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly SignInManager<Participant> _signInManager;
        private readonly ILogger<IdentifyModel> _logger;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;


        public IdentifyModel(SignInManager<Participant> signInManager,
            ILogger<IdentifyModel> logger,
            UserManager<Participant> userManager, Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }


        [BindProperty]
        public Participant UserDatas { get; set; }
        public string ProfileLink { get; set; }
        //public SEC GetSEC { get; set; }
        public bool NotFound { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                NotFound = true;
                return Page();
            }
            UserDatas = await _userManager.Users.Include(x=>x.SEC).FirstOrDefaultAsync(x => x.IdDigit == id);
            if (UserDatas == null)
            {
                NotFound = true;
                return Page();
            }
            //var queryableSource = _context.SECs.AsQueryable();
            //GetSEC = queryableSource.FirstOrDefault(x => x.Id == UserDatas.SECId);
            return Page();
        }

    }

}
