using Exwhyzee.AANI.Domain.Enums;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Event
    {
        public Event()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Toipc { get; set; }
        public string? InvitImage { get; set; }
        public string? Location { get; set; }

        public string? ImageUrl { get; set; }
        public string? ImageKey { get; set; }

        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Note { get; set; }
        public string? Report { get; set; }
        public EventStatus EventStatus { get; set; }
        //public decimal Budget { get; set; }

        public ICollection<EventAttendance> EventAttendances { get; set; }
        public ICollection<EventCommitte> EventCommittes { get; set; }
        public ICollection<EventComment> EventComments { get; set; }
        public ICollection<EventExpenditure> EventExpenditures { get; set; }
        public ICollection<EventBudget> EventBudgets { get; set; }
        public virtual FundCategory FundCategory { get; set; }

        // --- NEW: Link to OperationYear ---
        public long? OperationYearId { get; set; }
        public OperationYear? OperationYear { get; set; }

    }
}
