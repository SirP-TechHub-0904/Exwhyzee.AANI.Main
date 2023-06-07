using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Exwhyzee.AANI.Web.Pages.Shared.ViewComponents
{
    public class QouteViewComponent : ViewComponent
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public QouteViewComponent(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(DateTime date)
        {
            var quotes = await _context.QouteOfDays.ToListAsync();
            var random = new Random();
            var randomQuote = quotes[random.Next(0, quotes.Count)];
            ViewBag.q = randomQuote.Message;
            return View();
        }
    }
}
