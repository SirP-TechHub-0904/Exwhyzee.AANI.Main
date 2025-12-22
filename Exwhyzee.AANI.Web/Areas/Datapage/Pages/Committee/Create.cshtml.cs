using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Committee
{
    public class CreateModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public CreateModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Exwhyzee.AANI.Domain.Models.Committee Committee { get; set; } = new Exwhyzee.AANI.Domain.Models.Committee();

        // JSON hidden field containing selected participant IDs from the client
        [BindProperty]
        public string? SelectedParticipantIdsJson { get; set; }

        public void OnGet()
        {
            // nothing to do — participants loaded via AJAX
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Committee.CreatedAt = DateTime.UtcNow;
            _context.Committees.Add(Committee);
            await _context.SaveChangesAsync();

            // parse selected members
            List<string> selectedIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(SelectedParticipantIdsJson))
            {
                try
                {
                    selectedIds = JsonSerializer.Deserialize<List<string>>(SelectedParticipantIdsJson) ?? new List<string>();
                }
                catch { }
            }

            if (selectedIds.Any())
            {
                var participants = await _userManager.Users.Where(u => selectedIds.Contains(u.Id)).ToListAsync();
                int order = 1;
                foreach (var p in participants)
                {
                    _context.CommitteeMembers.Add(new CommitteeMember
                    {
                        CommitteeId = Committee.Id,
                        ParticipantId = p.Id,
                        Role = null,
                        Order = order++,
                        AddedById = User?.Identity?.Name,
                        AddedAt = DateTime.UtcNow
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Committee created.";
            return RedirectToPage("./Index");
        }
    }
}