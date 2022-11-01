using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class EventBudget
    {
        public EventBudget()
        {
            Date = DateTime.UtcNow.AddHours(1);
            UpdatedDate = DateTime.UtcNow.AddHours(1);

        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public decimal Amount { get; set; }
        public string? AssignTo { get; set; }
        public bool Disabled { get; set; }

        public DateTime Date { get; set; }
        public DateTime UpdatedDate { get; set; }

        public long EventId { get; set; }
        public Event Event { get; set; }
    }
}
