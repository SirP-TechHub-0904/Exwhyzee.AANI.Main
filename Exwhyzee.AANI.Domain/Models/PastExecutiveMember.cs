using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class PastExecutiveMember
    {
        public PastExecutiveMember()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;
        public DateTime Date { get; set; }
        public string? Position { get; set; }
      
        public long PastExecutiveYearId { get; set; } = default!;
        public PastExecutiveYear PastExecutiveYear { get; set; } = default!;
    }
}
