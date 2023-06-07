using Exwhyzee.AANI.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Exwhyzee.AANI.Domain.Models
{
    public class ContributorCategory
    {
        public ContributorCategory()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Type")]
        public ContributionType ContributionType { get; set; }
        public ICollection<Contributor> Contributors { get; set; } = default!;
    }
}
