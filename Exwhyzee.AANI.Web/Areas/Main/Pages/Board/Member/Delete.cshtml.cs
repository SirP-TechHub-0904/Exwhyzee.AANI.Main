using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Board.Member
{
    public class DeleteModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DeleteModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BoardOfGovornorMember BoardOfGovornorMember { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BoardOfGovornorMember = await _context.BoardOfGovornorMembers
                .Include(b => b.BoardOfGovornorCategory)
                .Include(b => b.Participant).FirstOrDefaultAsync(m => m.Id == id);

            if (BoardOfGovornorMember == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BoardOfGovornorMember = await _context.BoardOfGovornorMembers.FindAsync(id);

            if (BoardOfGovornorMember != null)
            {
                _context.BoardOfGovornorMembers.Remove(BoardOfGovornorMember);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
