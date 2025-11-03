using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class CampainYear
    {
        public long Id { get; set; }
        public int Year { get; set; } = 0;
        public ICollection<Campain> Campains { get; set; } = new List<Campain>();
    }
}
