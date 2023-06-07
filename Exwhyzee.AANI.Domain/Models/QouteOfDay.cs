using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class QouteOfDay
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
