namespace Exwhyzee.AANI.Domain.Models
{
    public class Paper
    {
        public Paper()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;
        public DateTime Date { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? File { get; set; }
        public int Year { get; set; }

        public long? EventId { get; set; }
        public Event Event { get; set; } = default!;

        public long PaperCategoryId { get; set; } = default!;
        public PaperCategory PaperCategory { get; set; } = default!;

        public string? CoverUrl { get; set; }
        public string? CoverKey { get; set; }
    }
}
