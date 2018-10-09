using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetInsight.Dto;

namespace AspNetInsight.Repo
{
    /// <summary>
    /// ResponseLog repository, that uses in-memory as data store
    /// </summary>
    public class CacheResponseLog : ILogRepo<ResponseLog>
    {
        static long _responseLogid = 1;

        ConcurrentBag<ResponseLog> ThisRepo
        {
            get
            {
                return MemoryStore.GetAspNetInsightRepo<ResponseLog>();
            }
        }

        public CacheResponseLog()
        {
            if (ThisRepo == null)
                MemoryStore.AddAspNetInsightRepo<ResponseLog>(new ConcurrentBag<ResponseLog>());
        }

        public IEnumerable<ResponseLog> GetByAppId(long appId)
        {
            var rtn = ThisRepo.Where(
                r => r.AppId == appId);

            if (rtn.Any())
                return rtn.DeepCopy();
            return default(IEnumerable<ResponseLog>);
        }

        public IEnumerable<ResponseLog> GetByAppId(long appId, DateTime from, DateTime to)
        {
            var rtn = ThisRepo.Where(
                r => r.AppId == appId && r.CreatedOn >= from && r.CreatedOn <= to);

            if (rtn.Any())
                return rtn.DeepCopy();
            return default(IEnumerable<ResponseLog>);
        }

        public void Log(ResponseLog dataToLog)
        {
            if (dataToLog == default(ResponseLog))
                throw new ArgumentNullException(nameof(dataToLog));

            var data = dataToLog.DeepCopy();

            data.Id = _responseLogid++;
            ThisRepo.Add(data);
        }
    }
}
