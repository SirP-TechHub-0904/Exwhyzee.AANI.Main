using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class PastExecutiveYear
    {
        public PastExecutiveYear()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }

        public ICollection<PastExecutiveMember> PastExecutiveMembers { get; set; } = default!;
    }
}
