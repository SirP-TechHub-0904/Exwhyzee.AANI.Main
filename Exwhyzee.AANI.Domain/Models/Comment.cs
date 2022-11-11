using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string? ParticipantId { get; set; }
        public Participant Participant { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public long BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
