using System;

namespace AspNetInsight.Dto
{
    /// <summary>
    /// Response Size details
    /// </summary>
    public class AppResponseSize : BasicSts
    {
        public long Id { get; set; }
        public long AppId { get; set; }
        public Size Scale { get; set; }
        public virtual App ApplicationDetails { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
