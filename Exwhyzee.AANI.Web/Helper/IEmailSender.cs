using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostmarkEmailService;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Helper
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendNotification(string toPhone, string message); 
        Task SendWhatsappAsync(string toPhone, string parameters, string buttonParameters = null, string headerParameters = null);

    }

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EmailSender> _logger;
        private readonly PostmarkClient _postmarkService;
        public EmailSender(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<EmailSender> logger, PostmarkClient postmarkService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _postmarkService = postmarkService;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {

            EmailResponseDto response = new EmailResponseDto();
            PostmarkResponse responsex = null;
            try
            {

              
                const string template = @"<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title></title>
</head>
<body style=""margin:0; padding:0; background-color:#f4f4f4;"">
  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color:#f4f4f4;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""max-width:600px; width:100%; margin:0 auto; background-color:#ffffff; border-collapse:collapse;"">
          <!-- Logo (centered) -->
          <tr>
            <td style=""padding:20px 24px 8px 24px; text-align:center;"">
              <img src=""https://aani.ng/img/logo.png"" alt=""Logo"" />
            </td>
          </tr>
 

          <!-- Main body space (insert contentHtml here) -->
          <tr>
            <td style=""padding:24px; font-family: Arial, 'Helvetica Neue', Helvetica, sans-serif; font-size:16px; color:#333333; line-height:1.5;"">
              {{2}}
            </td>
          </tr>
 <tr>
            <td style=""padding:18px 24px 30px 24px; font-family: Arial, 'Helvetica Neue', Helvetica, sans-serif; font-size:12px; color:#888888; text-align:center;"">
              <div style=""margin-bottom:8px;"">
                <a href=""https://aani.ng/"" style=""color:#888888; text-decoration:underline;"">Unsubscribe</a>
              </div>
              <div style=""margin-bottom:6px;"">
                <a href=""/"" style=""color:#888888; text-decoration:none;"">aanimni@gmail.com , aanimni@aol.com, aanimni@yahoo.com</a>
               |
                <span style=""color:#888888;"">AANI National Secretariat AANI House, Plot 417, Tigris Crecent, Opposite FCT High Court, Maitama, Abuja, Nigeria</span>
              </div>
              <div style=""color:#888888; margin-top:6px;"">© 2025 AANI. All rights reserved.</div>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";

                string newBody = template.Replace("{{2}}", htmlMessage);
                var message = new PostmarkMessage
                {
                    From = "AANI <admin@aani.ng>",

                    To = toEmail,
                    Subject = subject,
                    HtmlBody = newBody
                };


                responsex = await _postmarkService.SendMessageAsync(message);

                try
                {


                }
                catch (Exception ex) { }


            }
            catch (Exception ex)
            {
            }




        }

        public async Task SendNotification(string toPhone, string message)
        {
            if (string.IsNullOrWhiteSpace(toPhone)) throw new ArgumentException("toPhone must be provided", nameof(toPhone));
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("message must be provided", nameof(message));

            var token = _configuration["KudiSms:Token"];
            var senderId = _configuration["KudiSms:SenderId"];
            var gateway = _configuration["KudiSms:Gateway"] ?? "2";

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("KudiSms:Token is not configured.");

            if (string.IsNullOrWhiteSpace(senderId))
                throw new InvalidOperationException("KudiSms:SenderId is not configured.");

            try
            {
                var client = _httpClientFactory.CreateClient("kudi");

                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(token), "token");
                content.Add(new StringContent(senderId), "senderID");
                content.Add(new StringContent(toPhone), "recipients");
                content.Add(new StringContent(message), "message");
                content.Add(new StringContent(gateway), "gateway");

                // Post to the endpoint (base address should be configured when registering the HttpClient)
                var response = await client.PostAsync("api/sms", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("KudiSMS returned HTTP {StatusCode}: {Body}", response.StatusCode, body);
                    throw new HttpRequestException($"KudiSMS request failed with status {response.StatusCode}");
                }

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var kudiResp = JsonSerializer.Deserialize<KudiSmsResponse>(body, opts);

                if (kudiResp == null)
                {
                    _logger.LogError("Failed to deserialize KudiSMS response: {Body}", body);
                    throw new Exception("Invalid response from SMS gateway.");
                }

                if (!string.Equals(kudiResp.Status, "success", StringComparison.OrdinalIgnoreCase) || kudiResp.ErrorCode != "000")
                {
                    _logger.LogError("KudiSMS returned error: {ErrorCode} - {Msg} - Body: {Body}", kudiResp.ErrorCode, kudiResp.Msg, body);
                    throw new Exception($"SMS gateway returned error: {kudiResp.ErrorCode} - {kudiResp.Msg}");
                }

                _logger.LogInformation("SMS sent successfully. Cost={Cost} Balance={Balance} Data={Data}", kudiResp.Cost, kudiResp.Balance, string.Join(",", kudiResp.Data ?? Array.Empty<string>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {ToPhone}", toPhone);
                throw;
            }
        }

        public async Task SendWhatsappAsync(string toPhone, string parameters, string buttonParameters = null, string headerParameters = null)
        {
            string rex = "";
            try
            {
                var client = _httpClientFactory.CreateClient();
                string apiUrl = "https://my.kudisms.net/api/whatsapp_custom";
                string apiKey = _configuration["KudiSms:Token"];
                string templateCode = _configuration["KudiSms:TemplateCode"];
                string phoneId = _configuration["KudiSms:PhoneId"];

                var values = new List<KeyValuePair<string, string>>
                {
                    new ("token", apiKey),
                    new ("recipient", toPhone),
                    new ("phone_number_id", phoneId),
                    new ("template_code", templateCode),
                    new ("parameters", parameters ?? "")
                };

                if (!string.IsNullOrWhiteSpace(buttonParameters))
                    values.Add(new KeyValuePair<string, string>("button_parameters", buttonParameters));

                if (!string.IsNullOrWhiteSpace(headerParameters))
                    values.Add(new KeyValuePair<string, string>("header_parameters", headerParameters));

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("WhatsApp message sent! Response: {Result}", result);
                rex = result;
                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp message to {toPhone} using template {templateCode}");
               rex = ex.Message;
            }

           
        }

        private class KudiSmsResponse
        {
            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("error_code")]
            public string ErrorCode { get; set; }

            [JsonPropertyName("cost")]
            public string Cost { get; set; }

            [JsonPropertyName("data")]
            public string[] Data { get; set; }

            [JsonPropertyName("msg")]
            public string Msg { get; set; }

            [JsonPropertyName("length")]
            public int Length { get; set; }

            [JsonPropertyName("page")]
            public int Page { get; set; }

            [JsonPropertyName("balance")]
            public string Balance { get; set; }
        }

        public class EmailResponseDto
        {
            public NotificationStatus NotificationStatus { get; set; }
            public string Message { get; set; }
        }
    }
}