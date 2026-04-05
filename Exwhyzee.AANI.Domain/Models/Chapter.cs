using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Chapter
    {
        public long Id { get; set; }
        public string State { get; set; }
        public string Chairperson { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        // --- NEW ---
        public string? MonnifySubAccountCode { get; set; }      // e.g. MFY_SUB_XXXXXXXXX
        public string? MonnifyBankName { get; set; }            // e.g. "Access Bank"
        public string? MonnifyAccountNumber { get; set; }       // for display/reference
        public string? MonnifyAccountName { get; set; }         // for display/reference

        public ICollection<ChapterExecutive> ChapterExecutives { get; set; } = default!;
    }
}
