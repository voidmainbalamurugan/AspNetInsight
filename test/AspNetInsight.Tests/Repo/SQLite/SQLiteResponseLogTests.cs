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
    /// unit test cases for SQLiteResponseLog
    /// </summary>
    public class SQLiteResponseLogTests : Repo.InMemory.CacheResponseLogTests
    {
        public override ILogRepo<ResponseLog> NewLogRepo()
        {
            if (DefaultAppId == 0)
            {
                var app = Common.GetMockAppRepo().GetApp(1);
                app.Id = 0;
                var n = new SQLiteAppRepo().New(app);
                DefaultAppId = n.Id;
            }

            return new LogRepo();
        }
    }
}
