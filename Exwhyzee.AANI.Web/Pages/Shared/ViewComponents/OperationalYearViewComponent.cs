using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.ViewComponents
{
    public class OperationalYearViewComponent : ViewComponent
    {
        private readonly AaniDbContext _context;

        public OperationalYearViewComponent(AaniDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Find the single OperationYear that is marked as active.
            var activeYear = await _context.OperationYears
                .FirstOrDefaultAsync(oy => oy.IsActive);

            // Pass the single object directly to the view.
            return View(activeYear);
        }
    }
}