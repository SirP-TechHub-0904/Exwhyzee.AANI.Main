using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class EventComment
    {
        public EventComment()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
         
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public long EventId { get; set; }
        public Event Event { get; set; }
    }
}
