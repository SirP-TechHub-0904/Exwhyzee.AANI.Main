using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class PrincipalOfficer
    {
        public PrincipalOfficer()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string ParticipantId { get; set; } = default!;
        public Participant Participant { get; set; } = default!;

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime Date { get; set; }
        public long PrincipalOfficerCategoryId { get; set; } = default!;
        public PrincipalOfficerCategory PrincipalOfficerCategory { get; set; } = default!;
    }
}
