using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Pages.AlumniPage
{
    public class ListModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public ListModel(UserManager<Participant> userManager, Data.AaniDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IQueryable<Participant>? Participants { get; set; }

        public SEC SEC { get; set; }
        public async Task OnGetAsync(long id)
        {
            Participants = _userManager.Users.Include(x => x.SEC).Where(x=>x.SECId == id).AsQueryable();
            //await Task.Run();
            SEC = await _context.SECs.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
