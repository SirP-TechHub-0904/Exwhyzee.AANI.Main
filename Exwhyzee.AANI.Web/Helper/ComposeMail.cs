using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Helper
{
    //public class ComposeMail : IEmailSender
    //{
    //    private readonly string _smtpServer;
    //    private readonly int _smtpPort;
    //    private readonly string _smtpUsername;
    //    private readonly string _smtpPassword;

    //    public ComposeMail(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
    //    {
    //        _smtpServer = smtpServer;
    //        _smtpPort = smtpPort;
    //        _smtpUsername = smtpUsername;
    //        _smtpPassword = smtpPassword;
    //    }

    //    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    //    {
    //        using (var client = new SmtpClient(_smtpServer, _smtpPort))
    //        {
    //            client.UseDefaultCredentials = false;
    //            client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
    //            client.EnableSsl = true;

    //            var mailMessage = new MailMessage
    //            {
    //                From = new MailAddress(_smtpUsername),
    //                Subject = subject,
    //                Body = htmlMessage,
    //                IsBodyHtml = true
    //            };
    //            mailMessage.To.Add(email);

    //            await client.SendMailAsync(mailMessage);
    //        }
    //    }
    //}
}
