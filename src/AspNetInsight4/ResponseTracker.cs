using System;
using System.Web;
using AspNetInsight;
using AspNetInsight4.Module;

namespace AspNetInsight4
{
    /// <summary>
    /// AspNetInsight4 module that handles response instrumentations.
    /// </summary>
    public class ResponseTracker : IHttpModule
    {
        const string _contentType = "text/html";

        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
            _handler = null;
        }

        public void Init(HttpApplication context)
        {
            if (!InsightConfig.Instance.Enabled)
                return;

            context.BeginRequest += new EventHandler(OnBeginRequest);
            context.PostMapRequestHandler += new EventHandler(OnPostMapRequestHandler);
            context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
            context.PostRequestHandlerExecute += new EventHandler(OnPostRequestHandlerExecute);
            context.EndRequest += new EventHandler(OnEndRequest);
        }

        void OnBeginRequest(Object source, EventArgs e)
        {
            _handler.BeginRequest(source as HttpApplication);
        }
        
        void OnPostMapRequestHandler(Object source, EventArgs e)
        {
            _handler.HanldeDynamicContent(source as HttpApplication);
        }

        void OnPreRequestHandlerExecute(Object source, EventArgs e)
        {
            _handler.PreHanlderExecution(source as HttpApplication);
        }

        void OnPostRequestHandlerExecute(Object source, EventArgs e)
        {
            _handler.PostHanlderExecution(source as HttpApplication);
        }

        void OnEndRequest(Object source, EventArgs e)
        {
            var app = source as HttpApplication;
            _handler.LogResponseData(app);

            var canShow = InsightConfig.Instance.ShowBanner
                && app.Response.ContentType.Equals(_contentType, 
                   StringComparison.InvariantCultureIgnoreCase);
            if (canShow)
                _handler.UpdateResponse(source as HttpApplication);
        }

        #endregion

        IInstrumentationHanlder _handler { get; set; }

        public ResponseTracker()
        {
            InitHanlder(null);
        }

        public void InitHanlder(IInstrumentationHanlder handler)
        {
            _handler = handler ?? new AspNetInsightHanlder();
        }
        
    }
}
