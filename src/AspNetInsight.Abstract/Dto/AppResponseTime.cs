using System;

namespace AspNetInsight.Dto
{
    /// <summary>
    /// Application response time details
    /// </summary>
    public class AppResponseTime : BasicSts
    {
        public long Id { get; set; }
        public long AppId { get; set; }
        public TimeSlice Slice { get; set; }
        public virtual App ApplicationDetails { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
