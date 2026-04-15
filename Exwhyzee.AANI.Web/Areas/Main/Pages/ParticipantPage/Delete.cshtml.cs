using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    [Authorize(Roles = "mSuperAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public DeleteModel(AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public Participant Participant { get; set; } = default!;

        [BindProperty]
        public string ConfirmationText { get; set; } = string.Empty;

        public string ExpectedConfirmationText { get; set; } = string.Empty;

        public List<UsageInfo> Usages { get; set; } = new();

        public int TotalReferences => Usages.Sum(x => x.Count);

        public class UsageInfo
        {
            public string Location { get; set; } = string.Empty;
            public string Meaning { get; set; } = string.Empty;
            public int Count { get; set; }
            public List<string> Samples { get; set; } = new();
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var participant = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (participant == null)
            {
                return NotFound();
            }

            Participant = participant;
            ExpectedConfirmationText = BuildConfirmationText(participant.Email);
            Usages = await LoadUsagesAsync(participant.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var participant = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (participant == null)
            {
                return NotFound();
            }

            Participant = participant;
            ExpectedConfirmationText = BuildConfirmationText(participant.Email);
            Usages = await LoadUsagesAsync(participant.Id);

            if (!string.Equals(ConfirmationText?.Trim(), ExpectedConfirmationText, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, $"Confirmation text is incorrect. Type exactly: {ExpectedConfirmationText}");
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userId = participant.Id;

                //
                // DELETE DEPENDENT DATA FIRST
                //

                // Board of Governor
                _context.BoardOfGovornorMembers.RemoveRange(
                    _context.BoardOfGovornorMembers.Where(x => x.ParticipantId == userId));

                // Campaigns created by participant
                _context.Campains.RemoveRange(
                    _context.Campains.Where(x => x.ParticipantId == userId));

                // -----------------------------
                // Election-related deletes
                // -----------------------------

                // Accredited voter ids for this participant
                var accreditedVoterIds = await _context.ChapterAccreditedVoters
                    .Where(x => x.ParticipantId == userId)
                    .Select(x => x.Id)
                    .ToListAsync();

                // Vote ids tied to those accredited voters
                var voteIds = accreditedVoterIds.Any()
                    ? await _context.Votes
                        .Where(v => accreditedVoterIds.Contains(v.ChapterAccreditedVoterId))
                        .Select(v => v.Id)
                        .ToListAsync()
                    : new List<long>();

                // Election candidate ids tied to this participant
                var electionCandidateIds = await _context.ElectionCandidates
                    .Where(x => x.CandidateParticipantId == userId)
                    .Select(x => x.Id)
                    .ToListAsync();

                // Delete VoteChoices first
                if (voteIds.Any() || electionCandidateIds.Any())
                {
                    _context.VoteChoices.RemoveRange(
                        _context.VoteChoices.Where(x =>
                            voteIds.Contains(x.VoteId) ||
                            electionCandidateIds.Contains(x.CandidateId))
                    );

                    await _context.SaveChangesAsync();
                }

                // Delete Votes
                if (voteIds.Any())
                {
                    _context.Votes.RemoveRange(
                        _context.Votes.Where(x => voteIds.Contains(x.Id))
                    );

                    await _context.SaveChangesAsync();
                }

                // Delete ElectionCandidates
                if (electionCandidateIds.Any())
                {
                    _context.ElectionCandidates.RemoveRange(
                        _context.ElectionCandidates.Where(x => electionCandidateIds.Contains(x.Id))
                    );

                    await _context.SaveChangesAsync();
                }

                // Delete ChapterAccreditedVoters
                if (accreditedVoterIds.Any())
                {
                    _context.ChapterAccreditedVoters.RemoveRange(
                        _context.ChapterAccreditedVoters.Where(x => accreditedVoterIds.Contains(x.Id))
                    );

                    await _context.SaveChangesAsync();
                }

                // -----------------------------
                // Other dependent records
                // -----------------------------

                // Chapter executives
                _context.ChapterExecutives.RemoveRange(
                    _context.ChapterExecutives.Where(x => x.ParticipantId == userId));

                // Comments
                _context.Comments.RemoveRange(
                    _context.Comments.Where(x => x.ParticipantId == userId));

                // Committees
                _context.Committees.RemoveRange(
                    _context.Committees.Where(x => x.CreatedById == userId));

                _context.CommitteeMembers.RemoveRange(
                    _context.CommitteeMembers.Where(x => x.ParticipantId == userId));

                // Contributors
                _context.Contributors.RemoveRange(
                    _context.Contributors.Where(x => x.ParticipantId == userId));

                // Event attendance / comments / committee
                _context.EventAttendances.RemoveRange(
                    _context.EventAttendances.Where(x => x.ParticipantId == userId));

                _context.EventComments.RemoveRange(
                    _context.EventComments.Where(x => x.ParticipantId == userId));

                _context.EventCommittes.RemoveRange(
                    _context.EventCommittes.Where(x => x.ParticipantId == userId));

                // Executive / fund / login history / roles / papers
                _context.Executives.RemoveRange(
                    _context.Executives.Where(x => x.ParticipantId == userId));

                _context.Funds.RemoveRange(
                    _context.Funds.Where(x => x.ParticipantId == userId));

                _context.LoginHistories.RemoveRange(
                    _context.LoginHistories.Where(x => x.ParticipantId == userId));

                _context.OfficialRoles.RemoveRange(
                    _context.OfficialRoles.Where(x => x.ParticipantId == userId));

                _context.Papers.RemoveRange(
                    _context.Papers.Where(x => x.ParticipantId == userId));

                // Family links
                _context.ParticipantFamiliesOnSECs.RemoveRange(
                    _context.ParticipantFamiliesOnSECs.Where(x => x.ParticipantId == userId));

                // Past / Patron / Principal Officer
                _context.PastExecutiveMembers.RemoveRange(
                    _context.PastExecutiveMembers.Where(x => x.ParticipantId == userId));

                _context.Patrons.RemoveRange(
                    _context.Patrons.Where(x => x.ParticipantId == userId));

                _context.PrincipalOfficers.RemoveRange(
                    _context.PrincipalOfficers.Where(x => x.ParticipantId == userId));

                // Birthday messages
                _context.BirthdayMessages.RemoveRange(
                    _context.BirthdayMessages.Where(x => x.RecipientParticipantId == userId));

                // Notifications
                _context.Notifications.RemoveRange(
                    _context.Notifications.Where(x => x.CreatedById == userId));

                // Save remaining dependent deletions
                await _context.SaveChangesAsync();

                // -----------------------------
                // Remove ASP.NET Identity relations
                // -----------------------------
                var logins = await _context.UserLogins.Where(x => x.UserId == userId).ToListAsync();
                var roles = await _context.UserRoles.Where(x => x.UserId == userId).ToListAsync();
                var claims = await _context.UserClaims.Where(x => x.UserId == userId).ToListAsync();
                var tokens = await _context.UserTokens.Where(x => x.UserId == userId).ToListAsync();

                _context.UserLogins.RemoveRange(logins);
                _context.UserRoles.RemoveRange(roles);
                _context.UserClaims.RemoveRange(claims);
                _context.UserTokens.RemoveRange(tokens);

                await _context.SaveChangesAsync();

                // -----------------------------
                // Delete the participant account
                // -----------------------------
                var result = await _userManager.DeleteAsync(participant);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    await transaction.RollbackAsync();
                    return Page();
                }

                await transaction.CommitAsync();
                TempData["success"] = $"Participant {participant.Fullname} was deleted successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"Delete failed: {ex.Message}");
                return Page();
            }

        }

        private string BuildConfirmationText(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "unknown-walata";
            }

            var namePart = email.Split('@')[0].Trim().ToLowerInvariant();
            return $"{namePart}-walata";
        }

        private async Task<List<UsageInfo>> LoadUsagesAsync(string userId)
        {
            var list = new List<UsageInfo>();

            async Task AddUsageAsync<T>(
                IQueryable<T> query,
                string location,
                string meaning,
                Func<T, string>? sampleSelector = null)
            {
                var count = await query.CountAsync();
                if (count <= 0) return;

                var usage = new UsageInfo
                {
                    Location = location,
                    Meaning = meaning,
                    Count = count
                };

                if (sampleSelector != null)
                {
                    var items = await query.Take(5).ToListAsync();
                    usage.Samples = items.Select(sampleSelector).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                }

                list.Add(usage);
            }

            await AddUsageAsync(
                _context.BoardOfGovornorMembers.Where(x => x.ParticipantId == userId),
                "BoardOfGovornorMembers",
                "This user is recorded as a Board of Governor member.");

            await AddUsageAsync(
                _context.Campains.Where(x => x.ParticipantId == userId),
                "Campains",
                "This user is linked as the participant on a campaign record.");

            await AddUsageAsync(
                _context.ChapterAccreditedVoters.Where(x => x.ParticipantId == userId),
                "ChapterAccreditedVoters",
                "This user appears as an accredited voter.");

            await AddUsageAsync(
                _context.ChapterAccreditedVoters.Where(x => x.ParticipantId == userId),
                "ChapterAccreditedVoters (CandidateParticipantId)",
                "This user appears as a candidate/contester in election-related records.");

            await AddUsageAsync(
                _context.ChapterAccreditedVoters.Where(x => x.ParticipantId == userId),
                "ChapterAccreditedVoters (PerformedBy)",
                "This user performed an election-related action.");

            await AddUsageAsync(
                _context.ChapterExecutives.Where(x => x.ParticipantId == userId),
                "ChapterExecutives",
                "This user is linked as a chapter executive.");

            await AddUsageAsync(
                _context.Comments.Where(x => x.ParticipantId == userId),
                "Comments",
                "This user authored comments.");

            await AddUsageAsync(
                _context.Committees.Where(x => x.CreatedById == userId),
                "Committees",
                "This user is the committee owner/member on committee records.");

            await AddUsageAsync(
                _context.Committees.Where(x => x.CreatedById == userId),
                "Committees (RecipientParticipantId)",
                "This user is the recipient on committee records.");

            await AddUsageAsync(
                _context.CommitteeMembers.Where(x => x.ParticipantId == userId),
                "CommitteeMembers",
                "This user is listed as a committee member.");

            await AddUsageAsync(
                _context.Contributors.Where(x => x.ParticipantId == userId),
                "Contributors",
                "This user is linked as a contributor.");

            await AddUsageAsync(
                _context.EventAttendances.Where(x => x.ParticipantId == userId),
                "EventAttendances",
                "This user has attendance records for events.");

            await AddUsageAsync(
                _context.EventComments.Where(x => x.ParticipantId == userId),
                "EventComments",
                "This user wrote event comments.");

            await AddUsageAsync(
                _context.EventCommittes.Where(x => x.ParticipantId == userId),
                "EventCommittes",
                "This user is linked to an event committee.");

            await AddUsageAsync(
                _context.Executives.Where(x => x.ParticipantId == userId),
                "Executives",
                "This user is recorded as an executive.");

            await AddUsageAsync(
               _context.ElectionCandidates.Where(x => x.CandidateParticipantId == userId),
               "Election Candidates",
               "This user is recorded as an Election Candidates.");
            var electionCandidateIds = await _context.ElectionCandidates
    .Where(x => x.CandidateParticipantId == userId)
    .Select(x => x.Id)
    .ToListAsync();

            if (electionCandidateIds.Any())
            {
                await AddUsageAsync(
                    _context.VoteChoices.Where(x => electionCandidateIds.Contains(x.CandidateId)),
                    "VoteChoices (CandidateId)",
                    "This user is referenced in vote choices through election candidate records.");
            }

            await AddUsageAsync(
                _context.Funds.Where(x => x.ParticipantId == userId),
                "Funds",
                "This user is linked to fund records.");

            await AddUsageAsync(
                _context.LoginHistories.Where(x => x.ParticipantId == userId),
                "LoginHistories",
                "This user has login history/audit records.");

            await AddUsageAsync(
                _context.OfficialRoles.Where(x => x.ParticipantId == userId),
                "OfficialRoles",
                "This user holds or held an official role.");

            await AddUsageAsync(
                _context.Papers.Where(x => x.ParticipantId == userId),
                "Papers",
                "This user is linked to paper/document records.");

            await AddUsageAsync(
                _context.ParticipantFamiliesOnSECs.Where(x => x.ParticipantId == userId),
                "ParticipantFamiliesOnSECs",
                "This user is linked to family-on-SEC records.");

            await AddUsageAsync(
                _context.PastExecutiveMembers.Where(x => x.ParticipantId == userId),
                "PastExecutiveMembers",
                "This user appears as a past executive member.");

            await AddUsageAsync(
                _context.Patrons.Where(x => x.ParticipantId == userId),
                "Patrons",
                "This user appears as a patron.");

            await AddUsageAsync(
                _context.PrincipalOfficers.Where(x => x.ParticipantId == userId),
                "PrincipalOfficers",
                "This user appears as a principal officer.");

            await AddUsageAsync(
                _context.BirthdayMessages.Where(x => x.RecipientParticipantId == userId),
                "BirthdayMessages",
                "This user has generated/sent birthday message records.");


            await AddUsageAsync(
                _context.Notifications.Where(x => x.CreatedById == userId),
                "Notifications",
                "This user is linked to notifications.");

            // ASP.NET Identity tables
            await AddUsageAsync(
                _context.UserRoles.Where(x => x.UserId == userId),
                "AspNetUserRoles",
                "This user has assigned system roles.");

            await AddUsageAsync(
                _context.UserClaims.Where(x => x.UserId == userId),
                "AspNetUserClaims",
                "This user has identity claims.");

            await AddUsageAsync(
                _context.UserLogins.Where(x => x.UserId == userId),
                "AspNetUserLogins",
                "This user has external login records.");

            await AddUsageAsync(
                _context.UserTokens.Where(x => x.UserId == userId),
                "AspNetUserTokens",
                "This user has stored identity tokens.");

            return list.OrderBy(x => x.Location).ToList();
        }
    }
}