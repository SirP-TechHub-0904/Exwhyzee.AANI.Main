namespace Exwhyzee.AANI.Domain.Models
{
    public class PaperCategory
    {
        public PaperCategory()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }

        public ICollection<Paper> Papers { get; set; } = default!;
    }
}
