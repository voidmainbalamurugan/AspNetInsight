using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetInsight.Repo;
using AspNetInsight4.Repo;
using Xunit;

namespace AspNetInsight.Tests.Repo.SQLite
{
    /// <summary>
    /// unit test cases for SQLiteAppRepo
    /// </summary>
    public class SQLiteAppRepoTests : Repo.InMemory.CacheAppRepoTests
    {
        public override IAppRepo NewApp()
        {
            return new SQLiteAppRepo();
        }
        
    }
}
