namespace Exwhyzee.AANI.Domain.Models
{
    public class SEC
    {
        public SEC()
        {
            Date = DateTime.UtcNow.AddHours(1);
            
        }

        public long Id { get; set; }
        public string Number { get; set; } = default!;
        public int Year { get; set; }
        public string Title { get; set; } = default!;
        public DateTime Date { get; set; }
        public int SortOrder { get; set; }
        public ICollection<Participant> Participants { get; set; } = default!;
    }
}
