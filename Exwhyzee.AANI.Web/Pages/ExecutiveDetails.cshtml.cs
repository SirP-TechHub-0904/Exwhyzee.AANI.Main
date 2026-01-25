using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace Exwhyzee.AANI.Web.Pages
{
    public class ExecutiveDetailsModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;

        private readonly AaniDbContext _context;

        public ExecutiveDetailsModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Participant Participant { get; set; } = default!;
        public List<Executive> ExecutiveHistory { get; set; } = new();
        public List<PaperGroupViewModel> PaperGroups { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            // 1. Fetch Participant with all related info
            Participant = await _userManager.Users
                .Include(p => p.SEC)
                .Include(p => p.Chapter)
                .FirstOrDefaultAsync(p => p.Id == id);

          

            if (Participant == null) return NotFound();

            // 2. Fetch Executive History (All years they served)
            ExecutiveHistory = await _context.Executives
                .Include(e => e.ExecutivePosition)
                .Include(e => e.OperationYear)
                .Where(e => e.ParticipantId == id)
                .OrderByDescending(e => e.OperationYear.StartDate)
                .ToListAsync();

            // 3. Fetch and Group Papers by Category
            var papers = await _context.Papers
                .Include(p => p.PaperCategory)
                .Where(p => p.ParticipantId == id) // Assuming status check
                .ToListAsync();

            PaperGroups = papers
                .GroupBy(p => p.PaperCategory?.Title ?? "General")
                .Select(g => new PaperGroupViewModel
                {
                    CategoryName = g.Key,
                    Papers = g.OrderByDescending(x => x.Year).ToList()
                }).ToList();

            return Page();
        }
    }

    public class PaperGroupViewModel
    {
        public string CategoryName { get; set; }
        public List<Paper> Papers { get; set; }
    }
}