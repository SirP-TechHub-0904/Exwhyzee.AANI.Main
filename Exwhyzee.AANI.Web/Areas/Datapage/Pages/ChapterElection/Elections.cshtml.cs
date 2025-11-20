using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]
    public class ElectionsModel : PageModel
    {
        private readonly AaniDbContext _context;

        public ElectionsModel(AaniDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }

        public Chapter? Chapter { get; set; }

        public class ElectionRow
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public ElectionStatus Status { get; set; } 
            public int CandidatesCount { get; set; }
            public int TokensCount { get; set; }
            public int VotesCount { get; set; }
        }

        public List<ElectionRow> Elections { get; set; } = new List<ElectionRow>();

        public async Task<IActionResult> OnGetAsync(long chapterId)
        {
            ChapterId = chapterId;
            Chapter = await _context.Chapters.FindAsync(chapterId);
            if (Chapter == null) return NotFound();

            // load elections and counts
            Elections = await _context.ChapterElections
                .Where(e => e.ChapterId == chapterId)
                .OrderByDescending(e => e.StartAt)
                .Select(e => new ElectionRow
                {
                    Id = e.Id,
                    Title = e.Title,
                    StartAt = e.StartAt,
                    EndAt = e.EndAt,
                    CandidatesCount = e.Candidates.Count(),
                    TokensCount = _context.ChapterAccreditedVoters.Where(t => t.ChapterElectionId == e.Id).Count(),
                    VotesCount = _context.Votes.Where(v => v.ElectionId == e.Id).Count(),
                    Status = e.ElectionStatus
                })
                .ToListAsync();

            // compute status for each row (server time UTC)
             

            return Page();
        }
    }
}