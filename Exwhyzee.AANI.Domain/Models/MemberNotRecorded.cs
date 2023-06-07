using Exwhyzee.AANI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public  class MemberNotRecorded
    {
        public long Id { get; set; }
        public string? Surname { get; set; }
        public string? FirstName { get; set; }
        public string? OtherName { get; set; }
        public string? Title { get; set; }
        public string? Gender { get; set; }

        public string? State { get; set; } 
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Sponsor { get; set; }
         public string SecYear { get; set; }
         public string StudyGroup { get; set; }
         public string Position { get; set; }

        public string? DOB { get; set; }
    }
}
