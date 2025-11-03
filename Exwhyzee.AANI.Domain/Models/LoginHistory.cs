using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class LoginHistory
    {
        public long Id { get; set; }

        // FK to AspNetUsers (Participant.Id)
        public string ParticipantId { get; set; } = default!;
        public Participant Participant { get; set; } = default!;

        // When the login happened
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Optional metadata
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
