using System.ComponentModel;

namespace Exwhyzee.AANI.Domain.Enums
{

    public enum FundStatus
    {
        [Description("NONE")]
        NONE = 0,
        [Description("Active")]
        Active = 2,
        [Description("Changed")]
        Changed = 3,
        [Description("Remove")]
        Remove = 4

    }
}
