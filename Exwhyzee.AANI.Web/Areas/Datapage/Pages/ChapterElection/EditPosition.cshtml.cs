using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Authorize(Roles = "Admin,MNI")]
    public class EditPositionModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly ILogger<EditPositionModel> _logger;

        public EditPositionModel(AaniDbContext context, ILogger<EditPositionModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public long PositionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }

        [BindProperty]
        public PositionEditInput PositionEdit { get; set; } = new PositionEditInput();

        public class PositionEditInput
        {
            public long PositionId { get; set; }

            [Required, StringLength(250)]
            public string Name { get; set; } = string.Empty;

            [StringLength(150)]
            public string? Slug { get; set; }

            [StringLength(2000)]
            public string? Description { get; set; }

            public int Seats { get; set; } = 1;
            public int MaxChoices { get; set; } = 1;
            public int BallotOrder { get; set; } = 0;
            public bool IsActive { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync(long positionId, long electionId, long chapterId)
        {
            PositionId = positionId;
            ElectionId = electionId;
            ChapterId = chapterId;

            var pos = await _context.ElectionPositions.FirstOrDefaultAsync(p => p.Id == positionId && p.ElectionId == electionId);
            if (pos == null) return NotFound();

            PositionEdit = new PositionEditInput
            {
                PositionId = pos.Id,
                Name = pos.Name,
                Slug = pos.Slug,
                Description = pos.Description, 
                BallotOrder = pos.BallotOrder,
                IsActive = pos.IsActive
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input.";
                return Page();
            }

            var pos = await _context.ElectionPositions.FirstOrDefaultAsync(p => p.Id == PositionEdit.PositionId && p.ElectionId == ElectionId);
            if (pos == null)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToPage("/ChapterElection/Candidates", new { area = "Datapage", electionId = ElectionId, chapterId = ChapterId });
            }

            pos.Name = PositionEdit.Name;
            pos.Slug = PositionEdit.Slug;
            pos.Description = PositionEdit.Description; 
            pos.BallotOrder = PositionEdit.BallotOrder;
            pos.IsActive = PositionEdit.IsActive;
            pos.UpdatedAt = DateTime.UtcNow;

            _context.ElectionPositions.Update(pos);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Position '{pos.Name}' updated.";
            _logger.LogInformation("Position {PositionId} updated for election {ElectionId}", pos.Id, ElectionId);

            return RedirectToPage("/ChapterElection/Candidates", new { area = "Datapage", electionId = ElectionId, chapterId = ChapterId });
        }
    }
}