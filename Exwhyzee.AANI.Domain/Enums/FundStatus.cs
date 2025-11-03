using System.ComponentModel;

namespace Exwhyzee.AANI.Domain.Enums
{

    public enum FundStatus
    {
        [Description("NONE")]
        NONE = 0,
        [Description("Paid")]
        Paid = 2,
        [Description("Not Paid")]
        NotPaid = 3,
        [Description("Canceled")]
        Canceled = 4

    }
}
