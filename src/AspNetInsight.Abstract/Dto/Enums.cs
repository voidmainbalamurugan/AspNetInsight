using System.ComponentModel;

namespace AspNetInsight.Dto
{
    public enum Size
    {
        [Description("byte")]
        Byte = 1,
        [Description("K byte")]
        KB,
        [Description("MB")]
        MB,
        [Description("GB")]
        GB
    }

    public enum TimeSlice
    {
        [Description("MSec")]
        Milliseconds = 1,
        [Description("Sec")]
        Seconds
    }
}
