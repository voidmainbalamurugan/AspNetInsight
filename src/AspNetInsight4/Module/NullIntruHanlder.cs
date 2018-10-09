using AspNetInsight;
using AspNetInsight.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetInsight4.Module
{
    /// <summary>
    /// Null response handler used while tracking is disabled at site level
    /// </summary>
    public class NullIntruHanlder : IInstrumentationHanlder
    {
        public App CurrentApp { get; protected set; }
        public void LogResponseData(HttpApplication app)
        {
            // Method intentionally left empty.
        }
        public void PostHanlderExecution(HttpApplication app)
        {
            // Method intentionally left empty.
        }
        public void PreHanlderExecution(HttpApplication app)
        {
            // Method intentionally left empty.
        }
        public void UpdateResponse(HttpApplication app)
        {
            // Method intentionally left empty.
        }
        public void BeginRequest(HttpApplication app)
        {
            // Method intentionally left empty.
        }
        public void HanldeDynamicContent(HttpApplication app)
        {
            // Method intentionally left empty.
        }
    }
}
