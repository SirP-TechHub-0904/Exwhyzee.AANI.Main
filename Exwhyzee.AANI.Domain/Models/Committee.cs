using System;
using System.Collections.Generic;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Committee
    {
        public long Id { get; set; }

        // Human-friendly committee name (e.g., "National Executive Committee")
        public string Name { get; set; } = string.Empty;

        // Optional description/purpose
        public string? Description { get; set; }

        // Active timespan
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Audit
        public string? CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Members
        public ICollection<CommitteeMember> Members { get; set; } = new List<CommitteeMember>();
    }


    public class CommitteeMember
    {
        public long Id { get; set; }

        public long CommitteeId { get; set; }
        public Committee Committee { get; set; } = default!;

        // Participant/Identity user id
        public string ParticipantId { get; set; } = string.Empty;
        public Participant Participant { get; set; } = default!;

        // Role on the committee (Chair, Secretary, Member, etc.)
        public string? Role { get; set; }

        // Order for display
        public int Order { get; set; }

        // Audit
        public string? AddedById { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

    public class BirthdayTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; } // HTML
        public string? SmsBody { get; set; } // plain text
        public TimeSpan SendTime { get; set; } = TimeSpan.FromHours(9); // default 09:00
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    public enum BirthdayMessageStatus
    {
        Pending = 0,
        Processing = 1,
        Sent = 2,
        Failed = 3,
        Cancelled = 4
    }

    public class BirthdayMessage
    {
        public long Id { get; set; }
        public string RecipientParticipantId { get; set; } = ""; 
        public string Channel { get; set; } = ""; // "Email" or "Sms"
        public string? Subject { get; set; }
        public string Body { get; set; } = "";
        public DateTime ScheduledAt { get; set; } // UTC
        public DateTime ScheduledDate { get; set; } // Date portion (UTC midnight) for indexing/dedup
        public BirthdayMessageStatus Status { get; set; } = BirthdayMessageStatus.Pending;
        public int AttemptCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public string? Error { get; set; }
    }
}
