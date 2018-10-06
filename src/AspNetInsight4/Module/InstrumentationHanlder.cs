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
    /// Response handler used while tracking is enabled
    /// </summary>
    public class InstrumentationHanlder : IInstrumentationHanlder
    {
        private const string _prefix = "[T-";
        private const string _prefixSize = "[S-";
        private const string _suffix = "]";

        IResponseInstrumentation _responseIntru { get; set; }
        IResponseBanner _banner { get; set; }
        public App CurrentApp { get; protected set; }
        protected static InsightConfig _config
        {
            get
            {
                return InsightConfig.Instance;
            }
        }

        public InstrumentationHanlder(IResponseBanner banner = null)
            :this(new ProviderFactory().GetInstrumentRepoByProvider(_config.GetProvider())
                 , banner)
        {
            
        }

        public InstrumentationHanlder(IResponseInstrumentation responseInstru, IResponseBanner banner = null)
        {
            _responseIntru = responseInstru ?? throw new ArgumentNullException(nameof(responseInstru));
            _banner = banner ?? new HtmlBanner(_prefix, _suffix);

        }

        App Init()
        {
            var app = _responseIntru.AddApp(HttpContext.Current.GetAppDetails());

            return app;
        }

        public void LogResponseData(HttpApplication app)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));

            // Calculate the response time from response start time
            var rt = DateTime.Now.AspNetInsightNow() - app.Context.Timestamp.AspNetInsightNow();
            app.Context.Items.Add(InstrumentKeys._t_Response, rt);

            // Collect raw url
            var url = app.Request.RawUrl;

            // Get response size data
            var wrapper = app.Response.Filter as ResponseStreamWrapper;
            var byt = wrapper.Size;

            // Get App info
            CurrentApp = CurrentApp ?? Init();

            // form tracking data
            ResponseLog log = new ResponseLog()
            {
                AppId = CurrentApp.Id,
                ByteSent = byt,
                CreatedOn = DateTime.Now.AspNetInsightNow(),
                HanlderExeTime = Convert.ToDouble(app.Context.Items[InstrumentKeys._t_Handlder]),
                MachineName = CurrentApp.MachineName,
                RawUrl = url,
                ResponseTime = rt.TotalMilliseconds,
                Scale = Size.Byte,
                TimeScale = TimeSlice.Milliseconds,
                Url = CurrentApp.Url
            };
            
            // Log and get min, avg, max ..etc
            var time = _responseIntru.UpdateAndGet(log, out AppResponseSize size);

            // Generates the html widget data
            if(_config.ShowBanner)
                app.Context.Items.Add(InstrumentKeys._response_Banner, GenerateBanner(time, size));

        }

        public void PostHanlderExecution(HttpApplication app)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));

            app.Context.Items.Add(InstrumentKeys._et_Handler, DateTime.Now.AspNetInsightNow().Ticks);

            // Calculate handler exe time and update it in items collection
            app.Context.Items.Add(InstrumentKeys._t_Handlder,
                (TimeSpan.FromTicks(Convert.ToInt64(app.Context.Items[InstrumentKeys._et_Handler])) -
                 TimeSpan.FromTicks(Convert.ToInt64(app.Context.Items[InstrumentKeys._st_Handler]))
                 ).TotalMilliseconds);
        }

        public void PreHanlderExecution(HttpApplication app)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));

            app.Context.Items.Add(InstrumentKeys._st_Handler, DateTime.Now.AspNetInsightNow().Ticks);
        }

        /// <summary>
        /// Updates the html widget into response html
        /// </summary>
        /// <param name="app"></param>
        public void UpdateResponse(HttpApplication app)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));

            var banner = app.Context.Items[InstrumentKeys._response_Banner];

            if(banner != null)
            {
                app.Response.Write(banner);
                app.Response.Flush();
            }
        }

        public void BeginRequest(HttpApplication app)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));
            app.Context.Response.Filter = new ResponseStreamWrapper(app.Context.Response.Filter);
        }

        string GenerateBanner(AppResponseTime time, AppResponseSize size)
        {
            var preTxt = _banner.GeneratedBanner(time);
            var banner = new HtmlBanner(preTxt, _prefixSize, _suffix);

            return banner.GeneratedBanner(size);
        }

        /// <summary>
        /// Handles dynamic content using custom IHttpHandler
        /// </summary>
        /// <param name="app"></param>
        public void HanldeDynamicContent(HttpApplication app)
        {
            CssHandler.CanHandle(app);
        }
    }
}
