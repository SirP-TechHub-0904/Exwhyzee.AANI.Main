namespace Exwhyzee.AANI.Domain.Models
{
    public class BoardOfGovornorMember
    {
        public BoardOfGovornorMember()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; } = default!;
        public DateTime Date { get; set; }
        public string? Position { get; set; }
        public long BoardOfGovornorCategoryId { get; set; } = default!;
        public BoardOfGovornorCategory BoardOfGovornorCategory { get; set; } = default!;
    }
}
