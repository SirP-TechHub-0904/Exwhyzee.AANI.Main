using System.Text.Json;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Committee
{
    public class EditModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public EditModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Exwhyzee.AANI.Domain.Models.Committee Committee { get; set; } = new Exwhyzee.AANI.Domain.Models.Committee();

        // For dropdown preview in right card
        public List<ParticipantPreview> ParticipantsPreview { get; set; } = new();

        public class ParticipantPreview
        {
            public string Id { get; set; } = string.Empty;
            public string Fullname { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }

        // GET: load committee and a preview list of participants for dropdown
        public async Task<IActionResult> OnGetAsync(long id)
        {
            var c = await _context.Committees
                .Include(x => x.Members)
                .ThenInclude(m => m.Participant)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return NotFound();

            Committee = c;

            // load small preview (first 500 or configurable)
            var previews = await _userManager.Users
                .Where(u => u.MniStatus == Domain.Enums.MniStatus.MNI)
                .OrderBy(u => u.Surname) 
                .Select(u => new ParticipantPreview
                {
                    Id = u.Id,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.PhoneNumber
                })
                .ToListAsync();

            ParticipantsPreview = previews;
            return Page();
        }

        // POST: save committee fields
        public async Task<IActionResult> OnPostAsync()
        {
             

            var committee = await _context.Committees.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == Committee.Id);
            if (committee == null) return NotFound();

            committee.Name = Committee.Name;
            committee.Description = Committee.Description;
            committee.StartAt = Committee.StartAt;
            committee.EndAt = Committee.EndAt;
            committee.IsActive = Committee.IsActive;
            committee.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Committee updated.";
                return RedirectToPage("./Edit", new { id = committee.Id });
            }
            catch
            {
                ModelState.AddModelError("", "Failed saving committee.");
                await ReloadAndReturnPage();
                return Page();
            }
        }

        // POST: Add a participant to the committee (from dropdown)
        public async Task<IActionResult> OnPostAddMemberAsync(string committeeId, string participantId)
        {
            if (!long.TryParse(committeeId, out var cid) || string.IsNullOrWhiteSpace(participantId))
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return RedirectToPage("./Edit", new { id = Committee?.Id ?? cid });
            }

            var committee = await _context.Committees.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == cid);
            if (committee == null) return NotFound();

            // prevent duplicates
            if (committee.Members.Any(m => m.ParticipantId == participantId))
            {
                TempData["InfoMessage"] = "Participant is already a member.";
                return RedirectToPage("./Edit", new { id = cid });
            }

            var participant = await _userManager.FindByIdAsync(participantId);
            if (participant == null)
            {
                TempData["ErrorMessage"] = "Participant not found.";
                return RedirectToPage("./Edit", new { id = cid });
            }

            var maxOrder = committee.Members.Any() ? committee.Members.Max(m => m.Order) : 0;
            var cm = new CommitteeMember
            {
                CommitteeId = cid,
                ParticipantId = participantId,
                Role = null,
                Order = maxOrder + 1,
                AddedById = User?.Identity?.Name,
                AddedAt = DateTime.UtcNow
            };
            _context.CommitteeMembers.Add(cm);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Member added.";
            return RedirectToPage("./Edit", new { id = cid });
        }

        // POST: Remove member
        public async Task<IActionResult> OnPostRemoveMemberAsync(string committeeId, string participantId)
        {
            if (!long.TryParse(committeeId, out var cid) || string.IsNullOrWhiteSpace(participantId))
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return RedirectToPage("./Edit", new { id = Committee?.Id ?? 0 });
            }

            var mem = await _context.CommitteeMembers.FirstOrDefaultAsync(m => m.CommitteeId == cid && m.ParticipantId == participantId);
            if (mem == null)
            {
                TempData["InfoMessage"] = "Member not found.";
                return RedirectToPage("./Edit", new { id = cid });
            }

            _context.CommitteeMembers.Remove(mem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Member removed.";
            return RedirectToPage("./Edit", new { id = cid });
        }

        // POST: Edit member position/role (modal submit)
        public async Task<IActionResult> OnPostEditMemberAsync(string committeeId, string participantId, string position)
        {
            if (!long.TryParse(committeeId, out var cid) || string.IsNullOrWhiteSpace(participantId))
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return RedirectToPage("./Edit", new { id = Committee?.Id ?? 0 });
            }

            var mem = await _context.CommitteeMembers.FirstOrDefaultAsync(m => m.CommitteeId == cid && m.ParticipantId == participantId);
            if (mem == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToPage("./Edit", new { id = cid });
            }

            mem.Role = string.IsNullOrWhiteSpace(position) ? null : position;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Member updated.";
            return RedirectToPage("./Edit", new { id = cid });
        }

        // helper to reload preview when returning to page after errors
        private async Task ReloadAndReturnPage()
        {
            var c = await _context.Committees
                .Include(x => x.Members)
                .ThenInclude(m => m.Participant)
                .FirstOrDefaultAsync(x => x.Id == Committee.Id);

            Committee = c ?? Committee;

            var previews = await _userManager.Users
                .Where(u => u.MniStatus == Domain.Enums.MniStatus.MNI)
                .OrderBy(u => u.Surname)
                .Take(500)
                .Select(u => new ParticipantPreview
                {
                    Id = u.Id,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.PhoneNumber
                })
                .ToListAsync();

            ParticipantsPreview = previews;
        }
    }
}