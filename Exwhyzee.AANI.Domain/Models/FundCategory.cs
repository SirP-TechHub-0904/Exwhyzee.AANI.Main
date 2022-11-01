namespace Exwhyzee.AANI.Domain.Models
{
    public class FundCategory
    {
        public FundCategory()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public long? EventId { get; set; }
        public virtual Event Event { get; set; }

        public ICollection<Fund> Funds { get; set; }
    }
}
