using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class CampainPost
    {
        public long Id { get; set; }
        public long CampainId { get; set; }
        public Campain Campain { get; set; }

        public string? Url { get; set; }
        public string? Key { get; set; }

    }
}
