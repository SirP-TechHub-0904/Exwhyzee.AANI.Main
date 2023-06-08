using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Exwhyzee.AANI.Web.Pages.Shared.ViewComponents
{
    public class CampainListViewComponent : ViewComponent
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public CampainListViewComponent(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            var list = await _context.Campains.Where(x => x.ExecutivePositionId == id).ToListAsync();

            return View(list);
        }
    }
}
