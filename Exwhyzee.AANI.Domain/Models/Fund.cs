using Exwhyzee.AANI.Domain.Enums;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Fund
    {
        public Fund()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;
        public DateTime Date { get; set; }
        public DateTime DatePaid { get; set; }
       public FundStatus FundStatus { get; set; }
        public decimal Amount { get; set; }
        public long? EventId { get; set; }
        public Event Event { get; set; } = default!;

        public long FundCategoryId { get; set; } = default!;
        public FundCategory FundCategory { get; set; } = default!;

        // --- NEW: Link to OperationYear ---
        public long? OperationYearId { get; set; }
        public OperationYear? OperationYear { get; set; }
    }
}
