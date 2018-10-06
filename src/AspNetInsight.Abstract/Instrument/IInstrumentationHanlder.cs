using AspNetInsight.Dto;
using System.Web;

namespace AspNetInsight
{
    /// <summary>
    /// Handles the HttpApplication's instrumentation
    /// </summary>
    public interface IInstrumentationHanlder
    {
        App CurrentApp { get; }
        void BeginRequest(HttpApplication app);
        void HanldeDynamicContent(HttpApplication app);
        void PreHanlderExecution(HttpApplication app);
        void PostHanlderExecution(HttpApplication app);
        void LogResponseData(HttpApplication app);
        void UpdateResponse(HttpApplication app);
    }
}