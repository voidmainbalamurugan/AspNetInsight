using System;

namespace AspNetInsight.Dto
{
    /// <summary>
    /// App details
    /// </summary>
    public class App
    {
        public long Id { get; set; }
        public string MachineName { get; set; }
        public string AppName { get; set; }
        public string AppId { get; set; }
        public string Url { get; set; }
    }
}
