using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]
    public class SelectMembersModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly ILogger<SelectMembersModel> _logger;
        private readonly UserManager<Participant> _userManager;

        public SelectMembersModel(AaniDbContext context, ILogger<SelectMembersModel> logger, UserManager<Participant> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }
        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }

        public Chapter Chapter { get; set; } = default!;
        public Exwhyzee.AANI.Domain.Models.ChapterElection Election { get; set; } = default!;

        // View model used to render rows
        public class ParticipantRow
        {
            public string Id { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public bool IsAccredited { get; set; }
            public long? AccreditedRecordId { get; set; } // ChapterAccreditedVoter.Id if accredited
            public bool Voted { get; set; }
            public DateTime? TokenCreatedAt { get; set; }
            public DateTime? TokenSentAt { get; set; }
        }

        public List<ParticipantRow> Rows { get; set; } = new List<ParticipantRow>();

        // participant ids selected on form (Participant.Id is a string)
        [BindProperty]
        public string[]? SelectedParticipantIds { get; set; }

        // "Add" or "Remove"
        [BindProperty]
        public string ActionType { get; set; } = "Add";

        public async Task<IActionResult> OnGetAsync(long chapterId, long electionId)
        {
            ChapterId = chapterId;
            ElectionId = electionId;
            Chapter = await _context.Chapters.FindAsync(chapterId);
            if (Chapter == null) return NotFound();

            // Load accredited records for the chapter
            var accredited = await _context.ChapterAccreditedVoters
                .Where(a => a.ChapterId == chapterId && a.ChapterElectionId == electionId)
                .AsNoTracking()
                .ToListAsync();

            // Load participants (materialize to memory so we can safely use computed properties like FullName)
            // If you have many participants add server-side paging/search here
            var participants = await _userManager.Users
                .AsNoTracking()
                .OrderBy(p => p.Surname) // order by mapped column (Id). Change if you have FirstName/LastName mapped.
                .ToListAsync();

            // Build rows
            var accByParticipant = accredited
                .Where(a => a.ParticipantId != null)
                .ToDictionary(a => a.ParticipantId!, a => a);

            Rows = participants.Select(p =>
            {
                accByParticipant.TryGetValue(p.Id, out var acc);
                return new ParticipantRow
                {
                    Id = p.Id,
                    DisplayName = p is { } ? (p.GetType().GetProperty("Fullname") != null ? (string?)p.GetType().GetProperty("Fullname")!.GetValue(p) ?? p.Id : p.Id) ?? p.Id : p.Id,
                    Email = p.GetType().GetProperty("Email") != null ? (string?)p.GetType().GetProperty("Email")!.GetValue(p) : null,
                    Phone = p.GetType().GetProperty("PhoneNumber") != null ? (string?)p.GetType().GetProperty("PhoneNumber")!.GetValue(p) : null,
                    IsAccredited = acc != null,
                    AccreditedRecordId = acc?.Id,
                    Voted = acc?.Voted ?? false,
                    TokenCreatedAt = acc?.TokenCreatedAt,
                    TokenSentAt = acc?.TokenSentAt
                };
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedParticipantIds == null || SelectedParticipantIds.Length == 0)
            {
                TempData["Error"] = "No participants selected.";
                return RedirectToPage(new { chapterId = ChapterId });
            }

            // Normalize action type
            var action = (ActionType ?? "Add").Trim().ToLowerInvariant();

            Chapter = await _context.Chapters.FindAsync(ChapterId);
            if (Chapter == null) return NotFound();

            Election = await _context.ChapterElections.FindAsync(ElectionId);
            if (Election == null) return NotFound();

            var selected = SelectedParticipantIds.Distinct().ToArray();

            // We'll run inside a transaction to ensure consistency
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (action == "add")
                {
                    // Find existing accredited participant ids to skip duplicates
                    var existing = await _context.ChapterAccreditedVoters
                        .Where(a => a.ChapterId == ChapterId && selected.Contains(a.ParticipantId!))
                        .Select(a => a.ParticipantId!)
                        .ToListAsync();

                    var toAdd = selected.Where(id => !existing.Contains(id)).ToArray();
                    var createdCount = 0;

                    foreach (var pid in toAdd)
                    {
                        var newAcc = new ChapterAccreditedVoter
                        {
                            ParticipantId = pid,
                            ChapterId = ChapterId,
                            ChapterElectionId = ElectionId,
                            Date = DateTime.UtcNow,
                            Voted = false
                        };
                        _context.ChapterAccreditedVoters.Add(newAcc);
                        createdCount++;
                    }

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    TempData["Message"] = $"Added {createdCount} participant(s) to accreditation pool. {existing.Count} were already accredited and were skipped.";
                    _logger.LogInformation("Admin added {Count} accredited participants to chapter {ChapterId}", createdCount, ChapterId);
                }
                else if (action == "remove")
                {
                    // Find accredited records for selected participants in this chapter
                    var accList = await _context.ChapterAccreditedVoters
                        .Where(a => a.ChapterId == ChapterId && selected.Contains(a.ParticipantId!))
                        .ToListAsync();

                    var skipped = new List<string>();
                    var removedCount = 0;

                    foreach (var acc in accList)
                    {
                        if (acc.Voted)
                        {
                            // skip and report
                            var participant = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(p => p.Id == acc.ParticipantId);
                            var name = participant?.GetType().GetProperty("FullName") != null
                                ? (string?)participant.GetType().GetProperty("FullName")!.GetValue(participant) ?? acc.ParticipantId
                                : acc.ParticipantId;
                            skipped.Add(name ?? acc.ParticipantId ?? "Unknown");
                            continue;
                        }

                        // safe to remove
                        _context.ChapterAccreditedVoters.Remove(acc);
                        removedCount++;
                    }

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    if (skipped.Count == 0)
                    {
                        TempData["Message"] = $"Removed {removedCount} participant(s) from accreditation pool.";
                    }
                    else
                    {
                        TempData["Message"] = $"Removed {removedCount} participant(s). Skipped {skipped.Count} participant(s) who already voted and cannot be removed: {string.Join(", ", skipped)}";
                    }
                    _logger.LogInformation("Admin removed {Removed} accredited participants from chapter {ChapterId}; skipped {Skipped}", removedCount, skipped.Count, ChapterId);
                }
                else
                {
                    TempData["Error"] = "Unknown action.";
                    return RedirectToPage(new { chapterId = ChapterId, electionId = ElectionId });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Concurrency error when updating accreditation for chapter {ChapterId}", ChapterId);
                TempData["Error"] = "Concurrency error occurred. Please retry.";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error when updating accreditation for chapter {ChapterId}", ChapterId);
                TempData["Error"] = "An unexpected error occurred.";
            }

            return RedirectToPage(new { chapterId = ChapterId, electionId = ElectionId });
        }
    }
}