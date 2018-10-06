using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using AspNetInsight.Dto;

namespace AspNetInsight.Repo
{
    /// <summary>
    /// In memory store, that uses System.Web.Caching as data store
    /// </summary>
    internal static class MemoryStore
    {
        private static Cache _repo { get; set; }
        private const string _key_AppRepo = "_si_appRepo_";
        private static Dictionary<Type, string> _keys = new Dictionary<Type, string>()
        {
            { typeof(App), "_si_appRepo_" },
            { typeof(AppResponseTime), "_si_appRepo_response_" },
            { typeof(AppResponseSize), "_si_appRepo_size_" },
            { typeof(ResponseLog), "_si_appRepo_log_" }
        };

        public static Cache Repo
        {
            get
            {
                if (_repo == null)
                {
                    _repo = new Cache();
                }

                return _repo;
            }
        }

        public static ConcurrentBag<TRepo> GetAspNetInsightRepo<TRepo>()
        {
            if (_keys.TryGetValue(typeof(TRepo), out string key))
                return (ConcurrentBag<TRepo>)Repo.Get(key);

            return default(ConcurrentBag<TRepo>);
        }

        public static void AddAspNetInsightRepo<TRepo>(ConcurrentBag<TRepo> repo)
        {
            if (_keys.TryGetValue(typeof(TRepo), out string key))
                _repo.Insert(key, repo);
        }
    }
}
