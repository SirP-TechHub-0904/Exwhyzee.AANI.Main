using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Office
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long? CategoryId { get; set; }
        public OfficeCategory? Category { get; set; }
    }
}
