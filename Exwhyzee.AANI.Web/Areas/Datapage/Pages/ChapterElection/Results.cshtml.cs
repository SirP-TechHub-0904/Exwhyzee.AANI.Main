using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterElection
{
    [Authorize(Roles = "Admin,MNI")]

    public class ResultsModel : PageModel
    {
        private readonly AaniDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ResultsModel> _logger;
        private readonly IConfiguration _config;

        public ResultsModel(
            AaniDbContext db,
            IEmailSender emailSender,
            ILogger<ResultsModel> logger,
            IConfiguration config)
        {
            _db = db;
            _emailSender = emailSender;
            _logger = logger;
            _config = config;
        }

        // Inputs
        [BindProperty(SupportsGet = true)]
        public long ChapterId { get; set; }

        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }

        // Display
        public string ElectionTitle { get; set; } = string.Empty;
        public string ElectionPeriod { get; set; } = string.Empty;

        public int TotalAccredited { get; set; }
        public int TokensIssued { get; set; }
        public int VotesCast { get; set; }
        public double TurnoutPercent { get; set; }

        // Per-position results
        public List<PositionResult> PositionResults { get; set; } = new();

        // Unassigned candidates (PositionId == null)
        public List<CandidateResult> UnassignedCandidates { get; set; } = new();

        // DTOs
        public class PositionResult
        {
            public long PositionId { get; set; }
            public string PositionName { get; set; } = string.Empty;
            public string? PositionDescription { get; set; }
            public int Seats { get; set; } = 1;
            public int MaxChoices { get; set; } = 1;
            public int BallotOrder { get; set; } = 0;

            // total vote choices cast for candidates in this position
            public int TotalVotesForPosition { get; set; }

            public List<CandidateResult> Candidates { get; set; } = new();

            // winners (top Seats) - empty when no votes
            public List<CandidateResult> Winners { get; set; } = new();

            // whether a tie exists at the cutoff
            public bool HasTieAtCutoff { get; set; } = false;
        }

        public class CandidateResult
        {
            public long CandidateId { get; set; }
            public string CandidateName { get; set; } = string.Empty;
            public int Votes { get; set; }
            public double SharePercentOfPosition { get; set; } = 0.0; // candidateVotes / totalVotesForPosition * 100
            public int Rank { get; set; }
            public int BallotOrder { get; set; }
            public bool IsWinner { get; set; } = false;
            public long? PositionId { get; set; }
        }

        public async Task OnGetAsync(long electionId, long chapterId)
        {
            ChapterId = chapterId;
            ElectionId = electionId;

            // load election metadata
            var election = await _db.ChapterElections
                .AsNoTracking()
                .Where(e => e.Id == ElectionId && e.ChapterId == ChapterId)
                .Select(e => new { e.Id, e.Title, e.StartAt, e.EndAt })
                .FirstOrDefaultAsync();

            ElectionTitle = election?.Title ?? $"Election #{ElectionId}";
            ElectionPeriod = election != null ? $"{election.StartAt:yyyy-MM-dd} — {election.EndAt:yyyy-MM-dd}" : string.Empty;

            // totals
            TotalAccredited = await _db.ChapterAccreditedVoters
                .AsNoTracking()
                .CountAsync(a => a.ChapterElectionId == ElectionId && a.ChapterId == ChapterId);

            TokensIssued = await _db.ChapterAccreditedVoters
                .AsNoTracking()
                .CountAsync(a => a.ChapterElectionId == ElectionId && a.ChapterId == ChapterId && a.VoteTokenHash != null);

            // votes cast (count Vote records for this election where the accredited voter belongs to this chapter)
            VotesCast = await _db.Votes
                .AsNoTracking()
                .Where(v => v.ElectionId == ElectionId)
                .Join(_db.ChapterAccreditedVoters,
                      v => v.ChapterAccreditedVoterId,
                      a => a.Id,
                      (v, a) => new { Vote = v, Accredited = a })
                .CountAsync(x => x.Accredited.ChapterId == ChapterId);

            TurnoutPercent = TotalAccredited == 0 ? 0.0 : (double)VotesCast / TotalAccredited * 100.0;

            // Load positions with candidates and participant names
            var positions = await _db.ElectionPositions
                .AsNoTracking()
                .Where(p => p.ElectionId == ElectionId)
                .Include(p => p.Candidates)
                    .ThenInclude(c => c.CandidateParticipant)
                .OrderBy(p => p.BallotOrder)
                .ToListAsync();

            // Load all candidates for the election (to catch unassigned ones too)
            var allCandidates = await _db.ElectionCandidates
                .AsNoTracking()
                .Where(c => c.ElectionId == ElectionId)
                .Include(c => c.CandidateParticipant)
                .ToListAsync();

            // Load vote choices for this election where the accredited voter belongs to this chapter
            var voteChoicesQuery = _db.VoteChoices
                .AsNoTracking()
                .Where(vc => vc.Vote.ElectionId == ElectionId && vc.Vote.ChapterAccreditedVoter.ChapterId == ChapterId);

            var voteCounts = await voteChoicesQuery
                .GroupBy(vc => vc.CandidateId)
                .Select(g => new { CandidateId = g.Key, Votes = g.Count() })
                .ToListAsync();

            var votesByCandidate = voteCounts.ToDictionary(x => x.CandidateId, x => x.Votes);

            // For each position, compute candidate results
            PositionResults = new List<PositionResult>();
            foreach (var pos in positions)
            {
                var pr = new PositionResult
                {
                    PositionId = pos.Id,
                    PositionName = pos.Name,
                    PositionDescription = pos.Description, 
                    BallotOrder = pos.BallotOrder
                };

                var candidateResults = pos.Candidates
                    .Select(c =>
                    {
                        votesByCandidate.TryGetValue(c.Id, out var v);
                        return new CandidateResult
                        {
                            CandidateId = c.Id,
                            CandidateName = c.CandidateParticipant?.Fullname ?? c.CandidateParticipantId,
                            Votes = v,
                            BallotOrder = c.BallotOrder,
                            PositionId = pos.Id
                        };
                    })
                    .OrderByDescending(cr => cr.Votes)
                    .ThenBy(cr => cr.BallotOrder)
                    .ToList();

                pr.TotalVotesForPosition = candidateResults.Sum(x => x.Votes);

                // compute share and rank
                var rank = 1;
                foreach (var cr in candidateResults)
                {
                    cr.Rank = rank++;
                    cr.SharePercentOfPosition = pr.TotalVotesForPosition > 0 ? Math.Round(100.0 * cr.Votes / pr.TotalVotesForPosition, 1) : 0.0;
                }

                // Determine winners only if there are votes for this position.
                pr.Winners = new List<CandidateResult>();
                pr.HasTieAtCutoff = false;

                if (pr.TotalVotesForPosition > 0 && candidateResults.Any())
                {
                    var takeCount = Math.Min(candidateResults.Count, Math.Max(1, pr.Seats));
                    var winners = candidateResults.Take(takeCount).ToList();
                    pr.Winners = winners;
                    foreach (var w in winners) w.IsWinner = true;

                    // tie detection at cutoff (only meaningful when there is at least one vote)
                    if (pr.Seats > 0 && candidateResults.Count >= pr.Seats + 1)
                    {
                        var lastWinner = candidateResults.ElementAt(pr.Seats - 1);
                        var nextCandidate = candidateResults.ElementAt(pr.Seats);
                        pr.HasTieAtCutoff = lastWinner.Votes == nextCandidate.Votes;
                    }
                }

                pr.Candidates = candidateResults;
                PositionResults.Add(pr);
            }

            // Unassigned candidates
            UnassignedCandidates = allCandidates
                .Where(c => c.PositionId == null)
                .Select(c =>
                {
                    votesByCandidate.TryGetValue(c.Id, out var v);
                    return new CandidateResult
                    {
                        CandidateId = c.Id,
                        CandidateName = c.CandidateParticipant?.Fullname ?? c.CandidateParticipantId,
                        Votes = v,
                        BallotOrder = c.BallotOrder,
                        PositionId = null
                    };
                })
                .OrderByDescending(cr => cr.Votes)
                .ThenBy(cr => cr.BallotOrder)
                .ToList();

            // If you want share for unassigned candidates relative to their own total, compute here:
            var totalUnassignedVotes = UnassignedCandidates.Sum(c => c.Votes);
            var urank = 1;
            foreach (var uc in UnassignedCandidates)
            {
                uc.Rank = urank++;
                uc.SharePercentOfPosition = totalUnassignedVotes > 0 ? Math.Round(100.0 * uc.Votes / totalUnassignedVotes, 1) : 0.0;
                // Do not mark winners among unassigned if there are no votes
                uc.IsWinner = totalUnassignedVotes > 0 && uc.Rank <= 1; // keep default behavior only if votes exist (example)
            }
        }

        // Email results to all accredited voters (sends synchronously; consider background queue for large electorates)
        public async Task<IActionResult> OnPostEmailResultsAsync(long electionId, long chapterId)
        {
            ChapterId = chapterId;
            ElectionId = electionId;

            // recompute current results
            await OnGetAsync(electionId, chapterId);

            // build HTML results per-position
            var sb = new StringBuilder();
            sb.AppendLine($"<h3>Election results — {System.Net.WebUtility.HtmlEncode(ElectionTitle)}</h3>");
            sb.AppendLine($"<p>Total accredited: {TotalAccredited}<br/>Tokens issued: {TokensIssued}<br/>Votes cast: {VotesCast}<br/>Turnout: {TurnoutPercent:0.0}%</p>");

            foreach (var pr in PositionResults)
            {
                sb.AppendLine($"<h4>{System.Net.WebUtility.HtmlEncode(pr.PositionName)} (Seats: {pr.Seats})</h4>");
                sb.AppendLine($"<p>Total votes for position: {pr.TotalVotesForPosition}</p>");
                sb.AppendLine("<table border=\"1\" cellpadding=\"6\" cellspacing=\"0\" style=\"border-collapse:collapse;\">");
                sb.AppendLine("<thead><tr><th>#</th><th>Candidate</th><th>Votes</th><th>Share</th><th>Winner</th></tr></thead><tbody>");
                var idx = 1;
                foreach (var c in pr.Candidates)
                {
                    var shareText = pr.TotalVotesForPosition > 0 ? $"{c.SharePercentOfPosition:0.0}%" : "—";
                    sb.AppendLine($"<tr><td>{idx}</td><td>{System.Net.WebUtility.HtmlEncode(c.CandidateName)}</td><td>{c.Votes}</td><td>{shareText}</td><td>{(c.IsWinner ? "Yes" : "")}</td></tr>");
                    idx++;
                }
                sb.AppendLine("</tbody></table>");

                if (pr.TotalVotesForPosition == 0)
                {
                    sb.AppendLine("<p><em>No votes cast for this position — winner not declared.</em></p>");
                }
            }

            // include unassigned if any
            if (UnassignedCandidates.Any())
            {
                sb.AppendLine("<h4>Unassigned candidates</h4>");
                var idx = 1;
                sb.AppendLine("<table border=\"1\" cellpadding=\"6\" cellspacing=\"0\" style=\"border-collapse:collapse;\">");
                sb.AppendLine("<thead><tr><th>#</th><th>Candidate</th><th>Votes</th><th>Share</th></tr></thead><tbody>");
                foreach (var c in UnassignedCandidates)
                {
                    var shareText = UnassignedCandidates.Sum(x => x.Votes) > 0 ? $"{c.SharePercentOfPosition:0.0}%" : "—";
                    sb.AppendLine($"<tr><td>{idx}</td><td>{System.Net.WebUtility.HtmlEncode(c.CandidateName)}</td><td>{c.Votes}</td><td>{shareText}</td></tr>");
                    idx++;
                }
                sb.AppendLine("</tbody></table>");
            }

            sb.AppendLine("<p>— AANI Elections Admin</p>");

            var body = sb.ToString();
            var subject = $"Election results — {ElectionTitle}";

            // list recipients: accredited voters in this election/chapter with email
            var recipients = await _db.ChapterAccreditedVoters
                .AsNoTracking()
                .Where(a => a.ChapterElectionId == ElectionId && a.ChapterId == ChapterId && a.Participant != null && a.Participant.Email != null)
                .Include(a => a.Participant)
                .Select(a => new { a.Id, Email = a.Participant.Email })
                .ToListAsync();

            if (!recipients.Any())
            {
                TempData["Error"] = "No accredited voters with email addresses found.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var sent = 0;
            var failed = 0;

            // If the electorate is large, replace this loop with a background job to avoid timeouts and improve reliability.
            foreach (var r in recipients)
            {
                try
                {
                    await _emailSender.SendEmailAsync(r.Email!, subject, body);
                    sent++;
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger?.LogError(ex, "Failed to send results email to {Email} (accreditedId={Id}) for chapter={Chapter} election={Election}",
                        r.Email, r.Id, ChapterId, ElectionId);
                }
            }

            TempData["Message"] = $"Emailing completed. Sent: {sent}, Failed: {failed}.";
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}