namespace Exwhyzee.AANI.Domain.Models
{
    public class ParticipantFamiliesOnSEC
    {
        public ParticipantFamiliesOnSEC()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string ParticipantId { get; set; } = default!;
        public Participant Participant { get; set; } = default!; public DateTime Date { get; set; }

        public long SubCategoryFamiliesOnSECId { get; set; } = default!;
        public SubCategoryFamiliesOnSEC SubCategoryFamiliesOnSEC { get; set; } = default!;
    }
}
