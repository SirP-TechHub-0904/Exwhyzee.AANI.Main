using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Campain
    {
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;
        public DateTime Date { get; set; }
        
        public long? ExecutivePositionId { get; set; }
        public ExecutivePosition ExecutivePosition { get; set; }

        public string? Manifesto { get; set; }

        public ICollection<CampainPost> CampainPosts { get; set; }
    }
}
