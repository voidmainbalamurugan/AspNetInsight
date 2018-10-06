using AspNetInsight;
using AspNetInsight.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetInsight4.Module
{
    /// <summary>
    /// Handles AspNetInsight4 Response tracking
    /// </summary>
    public class AspNetInsightHanlder : IInstrumentationHanlder
    {
        IInstrumentationHanlder _instuHandler => GetHandler(_type);
        IInstrumentationHanlder _otherHanlder { get; set; }
        HanlderType _type { get; set; }
        
        public App CurrentApp { get; protected set; }

        protected static InsightConfig _config
        {
            get
            {
                return InsightConfig.Instance;
            }
        }

        public AspNetInsightHanlder(IResponseBanner banner = null)
            : this(new ProviderFactory().GetInstrumentRepoByProvider(_config.GetProvider())
                 , banner)
        { }

        public AspNetInsightHanlder(IResponseInstrumentation responseInstru, IResponseBanner banner = null)
        {
            _otherHanlder = new InstrumentationHanlder(responseInstru, banner);
            _type = HanlderType.Others;
        }

        public void LogResponseData(HttpApplication app) => _instuHandler.LogResponseData(app);
        public void PostHanlderExecution(HttpApplication app) => _instuHandler.PostHanlderExecution(app);
        public void PreHanlderExecution(HttpApplication app) => _instuHandler.PreHanlderExecution(app);
        public void UpdateResponse(HttpApplication app) => _instuHandler.UpdateResponse(app);
        public void BeginRequest(HttpApplication app) {

            var doNotTrack = app.Request.Url.ToString().EndsWith(CssHandler._url,
                    StringComparison.InvariantCultureIgnoreCase);

            CssHandler.CanMap(app);
            if (doNotTrack)
            {
                _type = HanlderType.NullType;
                return;
            }
            
            _type = HanlderType.Others;
            
            _instuHandler.BeginRequest(app);
        }

        public void HanldeDynamicContent(HttpApplication app)
        {
            CssHandler.CanHandle(app);
        }

        IInstrumentationHanlder GetHandler(HanlderType type)
        {
            return type == HanlderType.NullType ? new NullIntruHanlder() : _otherHanlder;
        }
    }
}
