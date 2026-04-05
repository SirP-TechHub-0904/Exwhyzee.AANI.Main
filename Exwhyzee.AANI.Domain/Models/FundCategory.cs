using Exwhyzee.AANI.Domain.Enums;

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

        // Payment rules
        public decimal? Amount { get; set; }           // null if flexible
        public bool IsFlexible { get; set; }           // member enters amount
        public bool IsNationalLevel { get; set; }      // true = all chapters see it
        public long? ChapterId { get; set; }           // null = national-owned
        public Chapter? Chapter { get; set; }

        // Split config
        public SplitType SplitType { get; set; }       // Percentage or FixedAmount
        public decimal NationalShare { get; set; }     // 0 = chapter keeps 100%
        public decimal ChapterShare { get; set; }

        public long? EventId { get; set; }
        public virtual Event Event { get; set; }

        public ICollection<Fund> Funds { get; set; }
    }
}
