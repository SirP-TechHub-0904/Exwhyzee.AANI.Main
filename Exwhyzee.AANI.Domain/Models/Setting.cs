using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class ContactSettingsModel
    {
        public long Id { get; set; }
        // Contact numbers (up to 3)
        public List<PhoneNumberInfo> PhoneNumbers { get; set; } = new List<PhoneNumberInfo>();

        // Email addresses with titles (up to 4)
        public List<EmailInfo> Emails { get; set; } = new List<EmailInfo>();

        // Addresses with titles (up to 4)
        public List<AddressInfo> Addresses { get; set; } = new List<AddressInfo>();

        // Social media links
        public SocialMediaLinks SocialMedia { get; set; } = new SocialMediaLinks();

        public string? AboutText { get; set; }
        public string? BreadcrumImageUrl { get; set; }
        public string? BreadcrumImageId { get; set; }


        public string? EventTitle { get; set; }
        public string? EventSubtitle { get; set; }
        public string? EventDescription { get; set; }

            public string? ExecutiveTitle { get; set; }
            public string? ExecutiveSubtitle { get; set; }
        public string? ExecutiveDescription { get; set; }

        public string? BlogTitle { get; set; }
        public string? BlogSubtitle { get; set; }
        public string? BlogDescription { get; set; }



    }
    public class PhoneNumberInfo
    {
        public long Id { get; set; }
        public string? Number { get; set; }
    }
    public class EmailInfo
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
    }

    public class AddressInfo
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Address { get; set; }
    }

    public class SocialMediaLinks
    {
        public long Id { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? Instagram { get; set; }
        public string? LinkedIn { get; set; }
        public string? Youtube { get; set; }
        // Add more as needed
    }
}
