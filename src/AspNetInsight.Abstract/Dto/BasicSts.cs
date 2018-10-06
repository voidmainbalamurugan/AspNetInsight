namespace AspNetInsight.Dto
{
    /// <summary>
    /// Basic summary of an Insight Data
    /// </summary>
    public class BasicSts
    {
        public long Total { get; set; }
        public double Min { get; set; }
        public double Avg { get; set; }
        public double Max { get; set; }
        public double Recent { get; set; }

        public BasicSts()
        {
            Min = -1;
            Avg = -1;
            Max = -1;
            Total = 0;
        }
        public BasicSts(double current)
        {
            Total = 1;
            Recent = current;
            Min = current;
            Max = current;
            Avg = current;
        }
    }
}
