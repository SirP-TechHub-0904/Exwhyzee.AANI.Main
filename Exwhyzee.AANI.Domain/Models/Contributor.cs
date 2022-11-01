namespace Exwhyzee.AANI.Domain.Models
{
    public class Contributor
    {
        public Contributor()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;

        public long ContributorCategoryId { get; set; }
        public ContributorCategory ContributorCategory { get; set; } = default!;
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public string Year { get; set; }

    }
}
