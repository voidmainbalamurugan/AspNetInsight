using System.Collections.Generic;
using AspNetInsight.Dto;

namespace AspNetInsight.Repo
{
    /// <summary>
    /// App details repository, that abstracts data store implementations 
    /// </summary>
    public interface IAppRepo
    {
        App New(App toAdd);
        App GetApp(long id);
        IEnumerable<App> GetAppByUrl(string url);
        App GetAppById(string appId);
        IEnumerable<App> GetAppByMachineName(string machineName);
    }
}
