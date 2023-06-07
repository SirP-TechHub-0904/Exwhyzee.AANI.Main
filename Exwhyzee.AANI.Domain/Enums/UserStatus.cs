using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Enums
{
    public enum UserStatus
    {
        [Description("NONE")]
        NONE = 0,
        [Description("Admin")]
        Admin = 2,
        [Description("Ordinary")]
        Ordinary = 3,
        [Description("MNI")]
        MNI = 4
    }
}
