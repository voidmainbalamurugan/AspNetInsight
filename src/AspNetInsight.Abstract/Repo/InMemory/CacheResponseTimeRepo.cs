using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetInsight.Dto;

namespace AspNetInsight.Repo
{
    /// <summary>
    /// ResponseTime repository, that uses in-memory as data store
    /// </summary>
    public class CacheResponseTimeRepo : IResponseRepo<AppResponseTime>
    {
        private static long _responseTimeid = 1;
        private IAppRepo _appRepo;

        private ConcurrentBag<AppResponseTime> ThisRepo
        {
            get
            {
                return MemoryStore.GetAspNetInsightRepo<AppResponseTime>();
            }
        }

        public CacheResponseTimeRepo(IAppRepo appRepo = null)
        {
            if (ThisRepo == null)
                MemoryStore.AddAspNetInsightRepo(new ConcurrentBag<AppResponseTime>());

            _appRepo = appRepo ?? new CacheAppRepo();
        }

        public AppResponseTime GetAppData(long id)
        {
            var rtime = ThisRepo.Where(
                rt => rt.Id == id);
            if (rtime.Any())
                return rtime.FirstOrDefault().DeepCopy();
            return null;
        }

        public AppResponseTime GetAppDataById(long appId)
        {
            var rtime = ThisRepo.Where(
                rt => rt.AppId == appId);
            if (rtime.Any())
                return rtime.FirstOrDefault().DeepCopy();
            return null;
        }

        public AppResponseTime AddAppData(AppResponseTime appData)
        {
            if (appData == default(AppResponseTime))
                throw new ArgumentNullException(nameof(appData));

            var data = appData.DeepCopy();
            data.Id = _responseTimeid++;

            ThisRepo.Add(data);
            return data;
        }

        public AppResponseTime UpdateRecentByAppId(long appId, double recentTime)
        {
            var app = _appRepo.GetApp(appId);
            if (app == null)
                throw new ArgumentException(nameof(appId));

            AppResponseTime rtn = null;
            lock(ThisRepo)
            {
                var cr = ThisRepo.Where(
                    rt => rt.AppId == appId
                    );

                if (!cr.Any())
                {
                    var newResponse = new AppResponseTime()
                    {
                        AppId = appId,
                        Id = _responseTimeid++,
                        ApplicationDetails = app,
                        Recent = recentTime,
                        Min = recentTime,
                        Avg = recentTime,
                        Max = recentTime,
                        Slice = TimeSlice.Milliseconds,
                        Total = 1,
                        ModifiedDate = DateTime.UtcNow
                    };

                    ThisRepo.Add(newResponse);
                    rtn = newResponse;
                }
                else
                {
                    rtn = cr.FirstOrDefault();
                    rtn.Update(recentTime);
                }
            }

            return rtn.DeepCopy();
        }
    }
}