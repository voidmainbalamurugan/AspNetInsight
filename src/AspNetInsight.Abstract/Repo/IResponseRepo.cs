namespace AspNetInsight.Repo
{
    /// <summary>
    /// Response data repository
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IResponseRepo<TData>
        where TData : class, new()
    {
        TData GetAppData(long id);
        TData GetAppDataById(long appId);
        TData AddAppData(TData appData);
        TData UpdateRecentByAppId(long appId, double recent);
    }
}
