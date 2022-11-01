namespace Exwhyzee.AANI.Domain.Models
{
    public class CategoryFamiliesOnSEC
    {
        public CategoryFamiliesOnSEC()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }

        public ICollection<SubCategoryFamiliesOnSEC> SubCategories { get; set; } = default!;
    }
}
