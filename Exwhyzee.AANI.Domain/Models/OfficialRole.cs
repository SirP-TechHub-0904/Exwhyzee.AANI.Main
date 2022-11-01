namespace Exwhyzee.AANI.Domain.Models
{
    public class OfficialRole
    {
        public OfficialRole()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Date { get; set; }

        public string ParticipantId { get; set; } = default!;
        public Participant Participant { get; set; } = default!;


    }
}
