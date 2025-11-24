using System;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Notification
    {
        public long Id { get; set; }


        // Rendered recipient info (copied at time of queuing)
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Rendered subject/content (final content after token replacement)
        public string? Subject { get; set; }
        public string Content { get; set; } = string.Empty;

        public MessageType MessageType { get; set; }


        // Retry and status
        public int Retries { get; set; } = 0;
        public int MaxRetries { get; set; } = 5;

        public Exwhyzee.AANI.Domain.Enums.NotificationStatus Status { get; set; } = Exwhyzee.AANI.Domain.Enums.NotificationStatus.Pending;
        public Exwhyzee.AANI.Domain.Enums.NotificationPath NotificationPath { get; set; } 

        public string? ResponseMessage { get; set; }

        public string? CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
        // When it was finally sent successfully
        public DateTime? SentAt { get; set; }
        public bool Sent { get;set;} = false;
    }
}