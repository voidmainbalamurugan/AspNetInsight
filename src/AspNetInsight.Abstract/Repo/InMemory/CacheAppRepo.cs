using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetInsight.Dto;

namespace AspNetInsight.Repo
{
    /// <summary>
    /// App details repository, that uses in-memory as data store
    /// </summary>
    public class CacheAppRepo : IAppRepo
    {
        static long _appid = 1;

        ConcurrentBag<App> ThisRepo
        {
            get
            {
                return MemoryStore.GetAspNetInsightRepo<App>();
            }
        }

        public CacheAppRepo()
        {
            if(ThisRepo == null)
                MemoryStore.AddAspNetInsightRepo(new ConcurrentBag<App>());
        }
        
        public App GetApp(long id)
        {
            var crtn = ThisRepo.Where(a => a.Id == id);
            if (crtn.Any())
                return crtn.FirstOrDefault().DeepCopy();

            return null;
        }

        public App GetAppById(string appId)
        {
            if (appId == null)
                throw new ArgumentNullException(nameof(appId));

            var crtn = ThisRepo.Where(
                a => a.AppId.Equals(appId, StringComparison.InvariantCultureIgnoreCase));
            if (crtn.Any())
                return crtn.FirstOrDefault().DeepCopy();

            return null;
        }

        public IEnumerable<App> GetAppByMachineName(string machineName)
        {
            if (machineName == null)
                throw new ArgumentNullException(nameof(machineName));

            var crtn = ThisRepo.Where(
                a => a.MachineName.Equals(machineName, StringComparison.InvariantCultureIgnoreCase)
                );
            if (crtn.Any())
                return crtn.DeepCopy();

            return default(IEnumerable<App>);
        }

        public IEnumerable<App> GetAppByUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var crtn = ThisRepo.Where(
                a => a.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));
            if (crtn.Any())
                return crtn.DeepCopy();

            return default(IEnumerable<App>);
        }

        public App New(App toAdd)
        {
            if (toAdd == default(App))
                throw new ArgumentNullException(nameof(toAdd));
            var app = toAdd.DeepCopy();

            app.Id = _appid++;
            ThisRepo.Add(app);
            return app;
        }
    }
}
