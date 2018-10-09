using AspNetInsight.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetInsight
{
    /// <summary>
    /// Generates the live widget for given template, prefix and suffix configurations.
    /// </summary>
    public abstract class ResponseBanner : IResponseBanner
    {
        public string TemplateText { get; protected set; }
        public string Prefix { get; protected set; }
        public string Suffix { get; protected set; }

        protected const string _prefix = "[";
        protected const string _suffix = "]";
        const string _ex_msg = "GetTemplateText can not be empty or null";

        protected ResponseBanner(string prefix = _prefix, string suffix = _suffix)
        {
            Prefix = string.IsNullOrWhiteSpace(prefix) ? _prefix
                : prefix;
            Suffix = string.IsNullOrWhiteSpace(suffix) ? _suffix
                : suffix;
        }

        protected abstract string GetTemplateText();

        /// <summary>
        /// Generates the live widget for given data
        /// </summary>
        /// <param name="data">live data to be used</param>
        /// <returns></returns>
        public virtual string GeneratedBanner(BasicSts Data)
        {
            TemplateText = this.GetTemplateText();

            if (string.IsNullOrWhiteSpace(TemplateText))
                throw new ArgumentNullException(_ex_msg);

            return UpdateTemplate(Data);
        }

        string UpdateTemplate(BasicSts data)
        {
            if (data == default(BasicSts))
                throw new ArgumentNullException(nameof(data));

            var text = string.Copy(TemplateText);

            foreach (var kvp in data.GetDataAsKvp())
            {
                text = text.Replace(Prefix + kvp.Key + Suffix, kvp.Value);
            }
            
            return text;
        }
        
    }
}
