using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.EventPage.EventManager
{
    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Event Event { get; set; }
        public FundCategory FundCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = await _context.Events
                .Include(x => x.FundCategory)
                .ThenInclude(x => x.Funds)
                .Include(x=>x.EventComments)
                .Include(x=>x.EventCommittes)
                
                .ThenInclude(x=>x.Participant)
                
                .ThenInclude(x=>x.SEC)
                .Include(x=>x.EventAttendances).Include(x=>x.EventBudgets)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (Event == null)
            {
                return NotFound();
            }

            FundCategory = await _context.FundCategories.Include(x => x.Funds).FirstOrDefaultAsync(x => x.EventId == id);
            return Page();
        }

        [BindProperty]
        public string CommentText { get; set; }

        [BindProperty]
        public long EventId { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                EventComment comment = new EventComment();
                comment.ParticipantId = user.Id;
                comment.EventId = EventId;
                comment.Comment = CommentText;
                comment.Date = DateTime.UtcNow.AddHours(1);
                _context.EventComments.Add(comment);
                await _context.SaveChangesAsync();
                TempData["aasuccess"] = "comment/review updated successfully";
            }
            catch (Exception)
            {
                TempData["aaerror"] = "Unable to Update";
            }
            return RedirectToPage("/EventPage/EventManager/Details", new {id = EventId});
        }
    }
}
