using System;

namespace AspNetInsight.Dto
{
    /// <summary>
    /// Raw - response log
    /// </summary>
    public class ResponseLog
    {
        public long Id { get; set; }
        public long AppId { get; set; }
        public string MachineName { get; set; }
        public string Url { get; set; }
        public Size Scale { get; set; }
        public double ResponseTime { get; set; }
        public TimeSlice TimeScale { get; set; }
        public double HanlderExeTime { get; set; }
        public double ByteSent { get; set; }
        public string RawUrl { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
