using Exwhyzee.AANI.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Buffers.Text;

namespace Exwhyzee.AANI.Web.Helper
{
    public class TokenService
    {
        private readonly AaniDbContext _db;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public TokenService(AaniDbContext db, IConfiguration config, IEmailSender emailSender)
        {
            _db = db;
            _config = config;
            _emailSender = emailSender;
        }

        // Generate a single 7-digit token for an accredited voter and optionally send email
        // Returns the plain token (admin should present it and NOT persist it in DB).
        public async Task<string> GenerateAndOptionallySendAsync(long accreditedId, long electionId, bool sendNow = true)
        {
            var accredited = await _db.ChapterAccreditedVoters
                .Include(a => a.Participant)
                .FirstOrDefaultAsync(a => a.Id == accreditedId);

            if (accredited == null) throw new ArgumentException("Accredited voter not found");

            // generate a 7-digit numeric token
            var plainToken = TokenHelper.GenerateNumericToken(7);

            // compute hash with secret
            var secret = _config["TokenSecret"] ?? throw new InvalidOperationException("TokenSecret not configured");
            var tokenHash = TokenHelper.ComputeHmacSha256(plainToken, secret);

            // Save hash and timestamps (do not store the plain token)
            accredited.VoteTokenHash = tokenHash;
            accredited.TokenCreatedAt = DateTime.UtcNow;
            accredited.TokenExpiresAt = DateTime.UtcNow.AddDays(1); // configurable
            accredited.TokenSentAt = null;
            _db.Update(accredited);
            await _db.SaveChangesAsync();

            if (sendNow)
            {
                var participant = accredited.Participant;
                if (!string.IsNullOrWhiteSpace(participant?.Email))
                {

                    // VotingService.cs (email sending snippet)
                    var baseUrl = _config["VotingBaseUrl"]?.TrimEnd('/') ?? "https://example.com";

                    // Link does NOT include the token
                    var voteUrl = $"{baseUrl}/vote/{electionId}";
                    var subject = $"AANI Voting Token for {accredited.Chapter?.ToString() ?? "your chapter"}";
                    var body =
                        $"Dear {participant?.Fullname ?? participant?.Id},<br/><br/>" +
                        $"Use this link to open the voting page: <a href=\"{voteUrl}\">{voteUrl}</a><br/><br/>" +
                        $"Then enter this 7-digit code on the voting page to cast your vote: <strong>{plainToken}</strong><br/><br/>" +
                        $"This code expires on {accredited.TokenExpiresAt:yyyy-MM-dd HH:mm} UTC.<br/><br/>" +
                        $"— AANI Elections";

                    try
                    {
                        await _emailSender.SendEmailAsync(participant.Email, subject, body);

                    }
                    catch (Exception ex)
                    {
                        // Log or handle email sending failure as needed
                        //throw new InvalidOperationException("Failed to send email", ex);
                    }
                    var perDigit = string.Join('-', plainToken.ToCharArray());
                    //var smsMessage = $"Your AANI digit is {perDigit}. Check your email for details. — AANI";
                    var smsMessage = $"Dear Distinguish, Check your email for the chapter EV0T1ING details. - AANI";
                    try
                    {
                        await _emailSender.SendNotification(participant.PhoneNumber, smsMessage);
                        accredited.TokenSentAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {

                        // TokenSentAt remains null to indicate it wasn't sent.
                    }

                    try
                    {
                        string parameters = $"{participant.Fullname},{plainToken}";  

                        await _emailSender.SendWhatsappAsync(participant.PhoneNumber, parameters);
                        accredited.TokenSentAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {

                        // TokenSentAt remains null to indicate it wasn't sent.
                    }
                    accredited.TokenSentAt = DateTime.UtcNow;
                    _db.Update(accredited);
                    await _db.SaveChangesAsync();
                }
            }

            return plainToken;
        }

        // Bulk generate without sending (you can then enqueue sends separately)
        // Returns mapping accreditedId -> plain token (present to admin once)
        public async Task<Dictionary<long, string>> GenerateBulkAsync(IEnumerable<long> accreditedIds, long electionId, int expiryDays = 1)
        {
            var ids = accreditedIds.ToArray();
            var list = await _db.ChapterAccreditedVoters
                .Include(a => a.Participant)
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();

            var secret = _config["TokenSecret"] ?? throw new InvalidOperationException("TokenSecret not configured");
            var result = new Dictionary<long, string>();

            foreach (var acc in list)
            {
                var plain = TokenHelper.GenerateNumericToken(7);
                var hash = TokenHelper.ComputeHmacSha256(plain, secret);

                acc.VoteTokenHash = hash;
                acc.TokenCreatedAt = DateTime.UtcNow;
                acc.TokenExpiresAt = DateTime.UtcNow.AddDays(expiryDays);
                acc.TokenSentAt = null;

                result[acc.Id] = plain;

                var participant = acc.Participant;
                if (!string.IsNullOrWhiteSpace(participant?.Email))
                {
                    // VotingService.cs (email sending snippet)
                    var baseUrl = _config["VotingBaseUrl"]?.TrimEnd('/') ?? "https://example.com";

                    // Link does NOT include the token
                    var voteUrl = $"{baseUrl}/vote/{electionId}";

                    var subject = $"AANI Voting Token for {acc.Chapter?.ToString() ?? "your chapter"}";
                    var body =
                        $"Dear {participant?.Fullname ?? participant?.Id},<br/><br/>" +
                        $"Use this link to open the voting page: <a href=\"{voteUrl}\">{voteUrl}</a><br/><br/>" +
                        $"Then enter this 7-digit code on the voting page to cast your vote: <strong>{plain}</strong><br/><br/>" +
                        $"This code expires on {acc.TokenExpiresAt:yyyy-MM-dd HH:mm} UTC.<br/><br/>" +
                        $"— AANI Elections";
                    try
                    {
                        await _emailSender.SendEmailAsync(participant.Email, subject, body);

                    }
                    catch (Exception ex)
                    {
                        // Log or handle email sending failure as needed
                        //throw new InvalidOperationException("Failed to send email", ex);
                    }
                    var perDigit = string.Join('-', plain.ToCharArray());
                    //var smsMessage = $"Your AANI digit is {perDigit}. Check your email for details. — AANI";
                    var smsMessage = $"Dear Distinguish, Check your email for the chapter EV0T1ING details. - AANI";

                    try
                    {
                        await _emailSender.SendNotification(participant.PhoneNumber, smsMessage);
                        acc.TokenSentAt = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {

                        // TokenSentAt remains null to indicate it wasn't sent.
                    }

                    try
                    {
                        string parameters = $"{participant.Fullname},{plain}";

                        await _emailSender.SendWhatsappAsync(participant.PhoneNumber, parameters);
                     }
                    catch (Exception ex)
                    {

                        // TokenSentAt remains null to indicate it wasn't sent.
                    }
                }
            }

            _db.UpdateRange(list);
            await _db.SaveChangesAsync();
            return result;
        }
    }
}
