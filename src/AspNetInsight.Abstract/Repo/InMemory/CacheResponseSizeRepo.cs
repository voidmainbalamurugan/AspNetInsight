using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetInsight.Dto;

namespace AspNetInsight.Repo
{
    /// <summary>
    /// ResponseSize repository, that uses in-memory as data store
    /// </summary>
    public class CacheResponseSizeRepo : IResponseRepo<AppResponseSize>
    {
        static long _responseSizeid = 1;
        IAppRepo _appRepo;

        ConcurrentBag<AppResponseSize> ThisRepo
        {
            get
            {
                return MemoryStore.GetAspNetInsightRepo<AppResponseSize>();
            }
        }

        public CacheResponseSizeRepo(IAppRepo appRepo = null)
        {
            if (ThisRepo == null)
                MemoryStore.AddAspNetInsightRepo(new ConcurrentBag<AppResponseSize>());

            _appRepo = appRepo ?? new CacheAppRepo();
        }

        public AppResponseSize GetAppData(long id)
        {
            var rtsize = ThisRepo.Where(
                rt => rt.Id == id);
            if (rtsize.Any())
                return rtsize.FirstOrDefault().DeepCopy();

            return default(AppResponseSize);
        }

        public AppResponseSize GetAppDataById(long appId)
        {
            var rtsize = ThisRepo.Where(
               rt => rt.AppId == appId);
            if (rtsize.Any())
                return rtsize.FirstOrDefault().DeepCopy();
            return default(AppResponseSize);
        }

        public AppResponseSize AddAppData(AppResponseSize appData)
        {
            if (appData == default(AppResponseSize))
                throw new ArgumentNullException(nameof(appData));

            var data = appData.DeepCopy();
            data.Id = _responseSizeid++;

            ThisRepo.Add(data);
            return data;
        }

        public AppResponseSize UpdateRecentByAppId(long appId, double recent)
        {
            var app = _appRepo.GetApp(appId);
            if (app == null)
                throw new ArgumentException(nameof(appId));

            AppResponseSize rtn = null;
            lock (ThisRepo)
            {
                var cr = ThisRepo.Where(
                    rt => rt.AppId == appId
                    );

                if (!cr.Any())
                {
                    var newResponse = new AppResponseSize()
                    {
                        AppId = appId,
                        Id = _responseSizeid++,
                        ApplicationDetails = app,
                        Recent = recent,
                        Min = recent,
                        Avg = recent,
                        Max = recent,
                        Scale = Size.Byte,
                        Total = 1,
                        ModifiedDate = DateTime.UtcNow
                    };

                    ThisRepo.Add(newResponse);
                    rtn = newResponse;
                }
                else
                {
                    rtn = cr.FirstOrDefault();
                    rtn.Update(recent);
                }
            }

            return rtn.DeepCopy();
        }
    }
}