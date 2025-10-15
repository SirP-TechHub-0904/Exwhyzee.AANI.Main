using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Blog
    {
        public Blog()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string SortOrder { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageKey { get; set; }
        public string? Source { get; set; }
        public string? Author { get; set; }
        public DateTime Date { get; set; }
        public long? BlogCategoryId { get; set; }
        public BlogCategory BlogCategory { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}