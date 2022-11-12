using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Board.Year
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public BoardOfGovornorCategory BoardOfGovornorCategory { get; set; }
        public IList<BoardOfGovornorMember> BoardOfGovornorMember { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BoardOfGovornorCategory = await _context.BoardOfGovornorCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (BoardOfGovornorCategory == null)
            {
                return NotFound();
            }
            BoardOfGovornorMember = await _context.BoardOfGovornorMembers
                .Include(p => p.Participant)
                .ThenInclude(p => p.SEC)
                .Include(p => p.BoardOfGovornorCategory).ToListAsync();
            return Page();
        }
    }
}
