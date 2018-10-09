using AspNetInsight;
using AspNetInsight.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetInsight4.Module
{
    /// <summary>
    /// Html widget generator used in AspNetInsight4
    /// </summary>
    public class HtmlBanner : ResponseBanner
    {
        string _template { get; set; }
        const string _defaultResourceName = "AspNetInsight4.Content.ResponseBanner.html";
        const string _url = "[CSS-URL]";

        protected override string GetTemplateText()
        {
            if (!string.IsNullOrWhiteSpace(_template))
                return _template;
            _template = _defaultResourceName.GetFromResouce();

            return _template;
        }

        public HtmlBanner(string prefix, string suffix)
            : base(prefix, suffix) { }

        public HtmlBanner(string template, string prefix, string suffix)
            : base(prefix, suffix) {
            _template = template;
        }
        
        /// <summary>
        /// Generates the widget html from configured template
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public override string GeneratedBanner(BasicSts Data)
        {
            var banner = base.GeneratedBanner(Data);
            banner = banner.Replace(_url, GetCurrentUrl());

            return banner;
        }

        protected virtual string GetCurrentUrl()
        {
            return string.Format("{0}://{1}/",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Authority);
        }
    }
}