using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetInsight.Repo
{
    /// <summary>
    /// Response Log repository
    /// </summary>
    /// <typeparam name="TLogData"></typeparam>
    public interface ILogRepo<TLogData>
        where TLogData : class, new()
    {
        void Log(TLogData dataToLog);
        IEnumerable<TLogData> GetByAppId(long appId);
        IEnumerable<TLogData> GetByAppId(long appId, DateTime from, DateTime to);
    }
}
