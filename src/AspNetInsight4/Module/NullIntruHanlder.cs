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
        public void LogResponseData(HttpApplication app) { }
        public void PostHanlderExecution(HttpApplication app) { }
        public void PreHanlderExecution(HttpApplication app) { }
        public void UpdateResponse(HttpApplication app) { }
        public void BeginRequest(HttpApplication app) { }
        public void HanldeDynamicContent(HttpApplication app) { }
    }
}
