using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class ChapterExecutive
    {
        public long Id { get; set; }
        public string? Position { get; set; }
        public string? ParticipantId { get; set; }
        public Participant? Participant { get; set; } = default!;

        public long? ChapterId { get; set; }
        public Chapter? Chapter { get; set; } = default!;
    }
}
