using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetInsight;
using AspNetInsight.Repo;
using AspNetInsight.Dto;
using System.Web;

namespace AspNetInsight4.Module
{
    /// <summary>
    /// Handles all response data store operations
    /// </summary>
    public class ResponseInstrumentation : IResponseInstrumentation
    {
        protected IAppRepo _appRepo { get; set; }
        protected IResponseRepo<AppResponseTime> _appResponse { get; set; }
        protected IResponseRepo<AppResponseSize> _appSize { get; set; }
        protected ILogRepo<ResponseLog> _log { get; set; }

        private ResponseInstrumentation() { }

        public ResponseInstrumentation(IAppRepo appRepo,
            IResponseRepo<AppResponseTime> appResponse,
            IResponseRepo<AppResponseSize> appSize,
            ILogRepo<ResponseLog> log)
        {
            _appRepo = appRepo ?? throw new ArgumentNullException(nameof(appRepo));
            _appResponse = appResponse ?? throw new ArgumentNullException(nameof(appResponse));
            _appSize = appSize ?? throw new ArgumentNullException(nameof(appSize));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public AppResponseTime UpdateAndGet(ResponseLog log, out AppResponseSize responseSize)
        {
            if (log == default(ResponseLog))
                throw new ArgumentNullException(nameof(log));

            var rtn = _appResponse.UpdateRecentByAppId(log.AppId, log.ResponseTime);
            responseSize = _appSize.UpdateRecentByAppId(log.AppId, log.ByteSent);

            _log.Log(log);

            return rtn;
        }
        
        public App AddApp(App toAdd)
        {
            if (toAdd == null)
                throw new ArgumentNullException(nameof(toAdd));

            return _appRepo?.New(toAdd);
        }
    }
}
