using System;
using System.Web;

namespace AspNetInsight4.Module
{
    /// <summary>
    /// Handles CSS files used in Html widget/template
    /// </summary>
    public class CssHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members
        const string _cssResourceName = "AspNetInsight4.Content.si-insight-banner.css";
        const string _contentType = "text/css";

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public static bool CanHandle(HttpApplication app)
        {
            return Handle(app, (ap) => { ap.Context.Handler = new CssHandler(); });           
        }

        public static bool CanMap(HttpApplication app)
        {
            return Handle(app, (ap) => { ap.Context.RemapHandler(new CssHandler()); });
        }

        static bool Handle(HttpApplication app, Action<HttpApplication> hanlde)
        {
            if (app == default(HttpApplication))
                throw new ArgumentNullException(nameof(app));

            if (!app.Request.Url.ToString().EndsWith(_url, StringComparison.InvariantCultureIgnoreCase))
                return false;

            hanlde(app);
            return true;
        }

        /// <summary>
        /// url pattern
        /// </summary>
        public static readonly string _url = "si-file-banner-css.ashx";

        /// <summary>
        /// Processes the request
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var cssContent = _cssResourceName.GetFromResouce();
            context.Response.ContentType = _contentType;
            context.Response.Write(cssContent);
            context.Response.Flush();
        }

        #endregion
    }
}
