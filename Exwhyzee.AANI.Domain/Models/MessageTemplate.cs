using System;

namespace Exwhyzee.AANI.Domain.Models
{
    public class MessageTemplate
    {
        public long Id { get; set; }

        // Human friendly name
        public string Name { get; set; } = string.Empty;

        // Email, SMS, Whatsapp, Mail
        public MessageType MessageType { get; set; }
         
        // Template body (HTML for email, plain text for SMS/Whatsapp)
        public string Body { get; set; } = string.Empty;
         

        public bool IsActive { get; set; } = true;

        public string? CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum MessageType
    {
        None = 0,
        Email = 1,
        Sms = 2,
        Whatsapp = 3,
     }
}