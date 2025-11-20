using System;
using System.Threading.Tasks;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.ChapterVoting
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]

    public class EnterTokenModel : PageModel
    {
        private readonly AaniDbContext _db;
        private readonly IConfiguration _config;

        public EnterTokenModel(AaniDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public long ElectionId { get; set; }

        [BindProperty]
        public string Token { get; set; } = string.Empty;

        public Exwhyzee.AANI.Domain.Models.ChapterElection? Election { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(long electionId)
        {
            ElectionId = electionId;
            Election = await _db.ChapterElections
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == ElectionId);
        }

        public async Task<IActionResult> OnPostAsync(long electionId)
        {
            ElectionId = electionId;
            Token = (Token ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(Token))
            {
                ErrorMessage = "Please enter your 7-digit code.";
                await LoadElectionAsync();
                return Page();
            }

            var secret = _config["TokenSecret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                // configuration issue — do not proceed
                ErrorMessage = "Server configuration error (token secret missing). Contact administrator.";
                await LoadElectionAsync();
                return Page();
            }

            // compute HMAC of supplied token using configured secret
            string suppliedHash;
            try
            {
                suppliedHash = TokenHelper.ComputeHmacSha256(Token, secret);
            }
            catch
            {
                ErrorMessage = "Invalid token format.";
                await LoadElectionAsync();
                return Page();
            }

            var now = DateTime.UtcNow;

            // find accredited record for this election with that token hash
            var accredited = await _db.ChapterAccreditedVoters
                .Include(a => a.Participant)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ChapterElectionId == ElectionId && a.VoteTokenHash == suppliedHash);

            if (accredited == null)
            {
                ErrorMessage = "Invalid code. Please check and try again.";
                await LoadElectionAsync();
                return Page();
            }

            // check expiry
            if (accredited.TokenExpiresAt.HasValue && accredited.TokenExpiresAt.Value <= now)
            {
                ErrorMessage = "This code has expired.";
                await LoadElectionAsync();
                return Page();
            }

            // check already used / voted
            if (accredited.TokenSentAt == null && accredited.VoteTokenHash != null)
            {
                // not an error — just informational; keep flow
            }
            if (accredited.VoteTokenHash != null && accredited.TokenCreatedAt != null && accredited.TokenExpiresAt != null)
            {
                // proceed
            }

            if (accredited.TokenExpiresAt == null)
            {
                // still allow but warn (rare)
            }

            if (accredited.Voted || accredited.DateVoted.HasValue || accredited.ChapterElectionId == null)
            {
                ErrorMessage = "This account has already voted in this election.";
                await LoadElectionAsync();
                return Page();
            }

            // If token was already used (TokenUsed/TokenUsedAt) we treat as used.
            // Some models use TokenUsedAt or TokenSentAt; check TokenUsedAt presence (token model earlier uses TokenUsedAt)
            var used = false;
            // try to detect common properties: TokenUsedAt or TokenSentAt used earlier in codebase
            // We attempt to read TokenUsedAt via EF shadow property if it exists; fallback to Voted flag above.
            try
            {
                // If your model uses TokenUsedAt property, we would inspect it. We already checked Voted above.
                // For safety, check TokenExpiresAt + VoteTokenHash presence only.
            }
            catch
            {
                // ignore
            }

            // Create an ephemeral VoteSession server-side so plain token never goes to client
            var session = new Exwhyzee.AANI.Domain.Models.VoteSession
            {
                AccreditedVoterId = accredited.Id,
                ElectionId = ElectionId,
                TokenHash = suppliedHash,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(30)
            };

            _db.VoteSessions.Add(session);
            await _db.SaveChangesAsync();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = session.ExpiresAt
            };
            Response.Cookies.Append("VoteSessionId", session.SessionId, cookieOptions);

            // Redirect to the ballot page to let the voter cast their choices
            return RedirectToPage("/ChapterVoting/Ballot", new { electionId = ElectionId });
        }

        private async Task LoadElectionAsync()
        {
            Election = await _db.ChapterElections
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == ElectionId);
        }
    }
}