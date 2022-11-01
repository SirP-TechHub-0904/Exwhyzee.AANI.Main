namespace Exwhyzee.AANI.Domain.Models
{
    public class PrincipalOfficerCategory
    {
        public PrincipalOfficerCategory()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }

        public ICollection<PrincipalOfficer> PrincipalOfficers { get; set; } = default!;
    }
}
