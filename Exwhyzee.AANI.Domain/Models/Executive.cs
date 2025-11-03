using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Executive
    {
         
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? ExecutivePositionId { get; set; }
        public ExecutivePosition ExecutivePosition { get; set; }

        // --- NEW: Link to OperationYear ---
        public long? OperationYearId { get; set; }
        public OperationYear? OperationYear { get; set; }

    }
}
