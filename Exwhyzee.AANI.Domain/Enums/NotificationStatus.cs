using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Enums
{
    public enum NotificationStatus
    {
        Pending = 0,
        Processing = 1,
        Sent = 2,
        Failed = 3
    }

    public enum NotificationPath
    {
        All = 0,
        Chapter = 1,
        SEC = 2,
     }
}
