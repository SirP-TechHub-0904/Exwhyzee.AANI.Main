using Exwhyzee.AANI.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Participant : IdentityUser
    {
        public Participant()
        {
            Date = DateTime.UtcNow.AddHours(1);
        }

        public string? Surname { get; set; }
        public string? FirstName { get; set; }
        public string? OtherName { get; set; }
        public string? Title { get; set; }
        public string? LGA { get; set; }
        public string? State { get; set; }
        public string? ContactAddress { get; set; }
        public string? HomeAddress { get; set; }
        public string? AltPhoneNumber { get; set; }
        public string? Sponsor { get; set; }
        public string? Bio { get; set; }
        public string? CurrentOffice { get; set; }
        public string? CurrentPosition { get; set; }
        public GenderStatus GenderStatus { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public ReligionStatus ReligionStatus { get; set; }
        public UserStatus UserStatus { get; set; }
        public MniStatus MniStatus { get; set; }
        public AliveStatus AliveStatus { get; set; }
        public ActiveStatus ActiveStatus { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        
        public DateTime DOB { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateUpdated { get; set; }
        public long SECId { get; set; }
        public SEC SEC { get; set; } = default!;

        public long? ChapterId { get; set; }
        public Chapter Chapter { get; set; } = default!;


        public long? MessageTemplateCategoryId { get; set; }
        public MessageTemplateCategory MessageTemplateCategory { get; set; } = default!;

        public ICollection<OfficialRole> OfficialRoles { get; set; } = default!;

        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactEmail { get; set; }


        public string? Fullname
        {
            get
            {
                return Surname + " " + FirstName + " " + OtherName;
            }
        }
        public string? PictureUrl { get; set; }
        public string? PictureKey { get; set; }
        public string? IdDigit { get; set; }
    }
}
