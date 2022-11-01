using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class EventExpenditure
    {
        public EventExpenditure()
        {
            Date = DateTime.UtcNow.AddHours(1);
            UpdatedExpenditureDate = DateTime.UtcNow.AddHours(1);

        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpenditureDate { get; set; }
        public DateTime UpdatedExpenditureDate { get; set; }
        public string? Note { get; set; }
        public decimal Amount { get; set; }

        public long EventId { get; set; }
        public Event Event { get; set; }
    }
}
