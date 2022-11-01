namespace Exwhyzee.AANI.Domain.Models
{
    public class SubCategoryFamiliesOnSEC
    {
        public SubCategoryFamiliesOnSEC()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }

        public long CategoryFamiliesOnSECId { get; set; } = default!;
        public CategoryFamiliesOnSEC CategoryFamiliesOnSEC { get; set; } = default!;

        public ICollection<ParticipantFamiliesOnSEC> ParticipantFamiliesOnSECs { get; set; } = default!;
    }
}
