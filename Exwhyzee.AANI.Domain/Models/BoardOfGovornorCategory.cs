namespace Exwhyzee.AANI.Domain.Models
{
    public class BoardOfGovornorCategory
    {
        public BoardOfGovornorCategory()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }

        public ICollection<BoardOfGovornorMember> BoardOfGovornorMembers { get; set; } = default!;
    }
}
