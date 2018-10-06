using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetInsight.Dto;
using AspNetInsight.Repo;
using AspNetInsight4.Repo;

namespace AspNetInsight.Tests.Repo.SQLite
{
    /// <summary>
    /// unit test cases for SQLiteResponseTime
    /// </summary>
    public class SQLiteResponseTimeRepoTests : Repo.InMemory.CacheResponseTimeRepoTests
    {
        public override IResponseRepo<AppResponseTime> NewResponseTimeRepo(IAppRepo repo = null)
        {
            if (DefaultAppId == 0)
            {
                var app = repo.GetApp(1);
                app.Id = 0;
                var n = new SQLiteAppRepo().New(app);
                DefaultAppId = n.Id;
            }

            return new SQLiteResponseTimeRepo();
        }
    }
}
