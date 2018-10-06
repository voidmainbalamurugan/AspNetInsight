using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;

namespace AspNetInsight
{
    /// <summary>
    /// Represents Insight configuration
    /// </summary>
    public sealed class InsightConfig
    {
        /// <summary>
        /// Default values used, when there is no configuration found on the website
        /// </summary>
        private static class Default
        {
            public const string _dataProvider = "SqLite";
            public const string _connection = "insight_data";
            public const string _responseTemplate = "default";
            public const string _enabled = "yes";
            public const string _showBanner = "no";
            public const string _sdb = "si_insight.sdb";
        }

        /// <summary>
        /// Configuration keys
        /// </summary>
        private static class Keys
        {
            public const string dataProvider = "DataProvider";
            public const string connection = "Connection";
            public const string responseTemplate = "ResponseTemplate";
            public const string enabled = "AspNetInsightEnabled";
            public const string showBanner = "ShowAspNetInsightBanner";
        }

        /// <summary>
        /// Data store provider
        /// </summary>
        public string DataProvider { get; private set; }

        /// <summary>
        /// Connection, for specified  data provider
        /// </summary>
        public string Connection { get; private set; }

        /// <summary>
        /// Connection string, for specified  data provider
        /// </summary>
        public string ConnectionString => _connectionPath;

        /// <summary>
        /// Template used to have site specific Html widget
        /// </summary>
        public string ResponseTemplate { get; private set; }

        /// <summary>
        /// Flag that represents the Tracking status
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Provider enum from configured value
        /// </summary>
        /// <returns></returns>
        public Provider GetProvider() => (Provider)Enum.Parse(typeof(Provider), DataProvider, true);

        /// <summary>
        /// If html widget to be shown pages
        /// </summary>
        public bool ShowBanner { get; set; }

        string _connectionPath { get; set; }
        private string GetConnectionPath(string path)
        {
            var _path = path;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        Default._connection);

                    Directory.CreateDirectory(folder);
                    _path = folder;
                }
            }

            var dp = (Provider)Enum.Parse(typeof(Provider), DataProvider, true);
            var fileName = Default._sdb;
            if (dp == Provider.InMemory)
                return dp.ToString();

            return string.Format("{0}{1}{2}", _path, Path.DirectorySeparatorChar, fileName);
        }
        
        private InsightConfig()
        {
            DataProvider = GetConfigOr(Keys.dataProvider, Default._dataProvider);
            Connection = GetConfigOr(Keys.connection, "");
            ResponseTemplate = GetConfigOr(Keys.responseTemplate, Default._responseTemplate);
            Enabled = GetConfigOr(Keys.enabled, Default._enabled)
                .Equals("yes", StringComparison.InvariantCultureIgnoreCase);
            ShowBanner = GetConfigOr(Keys.showBanner, Default._showBanner)
                .Equals("yes", StringComparison.InvariantCultureIgnoreCase);

            _connectionPath = GetConnectionPath(Connection);
        }

        private string GetConfigOr(string key, string _default)
        {
            var val = WebConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(val) ? _default : val;
        }

        private static InsightConfig _instance { get; set; }

        /// <summary>
        /// Singleton instance of Insight config
        /// </summary>
        public static InsightConfig Instance
        {
            get
            {
                if (_instance == default(InsightConfig))
                    _instance = new InsightConfig();

                return _instance;
            }
        }
    }

}
