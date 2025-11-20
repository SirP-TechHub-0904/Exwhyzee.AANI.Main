using System.ComponentModel;

namespace Exwhyzee.AANI.Domain.Enums
{

    public enum UserStatus
    {
        [Description("NONE")]
        NONE = 0,
        [Description("Active")]
        Active = 4,
        

    }
    public enum ElectionStatus
    {
        [Description("None")]
        None = 0,

        [Description("Draft")]
        Draft = 1,         // created but not published

        [Description("Upcoming")]
        Upcoming = 2,     // scheduled, not yet open

        [Description("Open")]
        Open = 3,         // currently accepting votes

        [Description("Closed")]
        Closed = 4,       // finished / voting closed

        [Description("Cancelled")]
        Cancelled = 5     // cancelled/voided election
    }
}
