using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class ExecutivePosition
    {
        public long Id { get; set; }
        public string Position { get; set; }

        public ICollection<Campain> Campains { get; set; }
    }
}
