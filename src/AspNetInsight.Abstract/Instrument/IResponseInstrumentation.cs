using AspNetInsight.Dto;

namespace AspNetInsight
{
    /// <summary>
    /// Response tracking handler
    /// </summary>
    public interface IResponseInstrumentation
    {
        AppResponseTime UpdateAndGet(ResponseLog log, out AppResponseSize responseSize);
        App AddApp(App toAdd);
    }
}