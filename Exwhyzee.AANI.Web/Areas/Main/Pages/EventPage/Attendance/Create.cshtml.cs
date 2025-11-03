using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.Attendance
{
    [Microsoft.AspNetCore.Authorization.Authorize]
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
        public Event Event { get; set; }

        // This will hold the list of members for the checklist
        public List<AttendeeChecklistItem> Checklist { get; set; }

        // This will receive the list of IDs from the checkboxes on submit
        [BindProperty]
        public List<string> SelectedParticipantIds { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            Event = await _context.Events.FindAsync(id);
            if (Event == null)
            {
                return NotFound();
            }

            // 1. Get IDs of participants already marked as present for this event
            var presentIds = await _context.EventAttendances
                .Where(ea => ea.EventId == id)
                .Select(ea => ea.ParticipantId)
                .ToListAsync();

            // 2. Get all MNI participants who are NOT in the 'present' list
            var absentParticipants = await _userManager.Users
                .Include(u => u.SEC)
                .Where(u => u.MniStatus == Domain.Enums.MniStatus.MNI && !presentIds.Contains(u.Id))
                .OrderBy(u => u.Surname)
                .ToListAsync();

            // 3. Create the checklist for the view
            Checklist = absentParticipants.Select(p => new AttendeeChecklistItem
            {
                ParticipantId = p.Id,
                FullName = $"{p.Surname?.ToUpper()} {p.FirstName?.ToUpper()} {p.OtherName?.ToUpper()}",
                Details = $"(SEC {p.SEC?.Number} - {p.SEC?.Year})"
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            if (SelectedParticipantIds == null || !SelectedParticipantIds.Any())
            {
                // If no one was selected, just go back to the attendance page
                return RedirectToPage("/EventPage/EventManager/AttendancePage", new { id });
            }

            var newAttendances = SelectedParticipantIds.Select(participantId => new EventAttendance
            {
                EventId = id,
                ParticipantId = participantId,
                DatetimeArrival = DateTime.UtcNow, // Automatically set arrival time
                Tag = "Present" // Default tag
            });

            await _context.EventAttendances.AddRangeAsync(newAttendances);
            await _context.SaveChangesAsync();

            TempData["aasuccess"] = $"{SelectedParticipantIds.Count} member(s) checked in successfully.";
            return RedirectToPage("/EventPage/EventManager/AttendancePage", new { id });
        }
    }

    // A simple ViewModel for the checklist items
    public class AttendeeChecklistItem
    {
        public string ParticipantId { get; set; }
        public string FullName { get; set; }
        public string Details { get; set; }
    }
}