using System.ComponentModel;

namespace Exwhyzee.AANI.Domain.Enums
{

    public enum MaritalStatus
    {
        [Description("NONE")]
        NONE = 0,
        [Description("Single")]
        Single = 2,
        [Description("Married")]
        Married = 3,
        [Description("Widowed")]
        Widowed = 4,
        [Description("Divorced")]
        Divorced = 5,
             [Description("Separated")]
        Separated = 6

    }
}
