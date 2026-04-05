using System.ComponentModel;

namespace Exwhyzee.AANI.Domain.Enums
{

    public enum EventStatus
    {
        [Description("NONE")]
        NONE = 0,
        [Description("Active")]
        Active = 2,
        [Description("Closed")]
        Closed = 3,
        [Description("Awaiting")]
        Awaiting = 4

    }

    public enum SplitType
    {
        Percentage = 1,
        FixedAmount = 2
    }
}
