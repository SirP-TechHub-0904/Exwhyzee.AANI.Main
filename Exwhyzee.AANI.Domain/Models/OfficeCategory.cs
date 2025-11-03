using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class OfficeCategory
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Office>? Offices { get; set; }
    }
}
