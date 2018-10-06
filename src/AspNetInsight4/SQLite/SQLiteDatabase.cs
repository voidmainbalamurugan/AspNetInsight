using AspNetInsight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Data.SQLite.Generic;
using AspNetInsight.Dto;
using System.IO;
using AspNetInsight.Data;
using System.Data.Common;

namespace AspNetInsight4.SQLite
{
    /// <summary>
    /// Represents SQLite data store
    /// </summary>
    internal sealed class SQLiteDatabase : Database
    {
        static string _dbFile { get; set; }
        static string _cs { get; set; }

        public SQLiteDatabase()
            :base(typeof(SQLiteDatabase))
        {

        }

        protected override void CreateDb()
        {
            _dbFile = InsightConfig.Instance.ConnectionString;
            if (!File.Exists(_dbFile))
            {
                SQLiteConnection.CreateFile(_dbFile);
                new FileInfo(_dbFile).GrantAccessToEveryone();
            }
            _cs = new SQLiteConnectionStringBuilder()
            {
                DataSource = _dbFile,
                DateTimeKind = DateTimeKind.Utc,
                Pooling = true,
                ForeignKeys = true
            }.ConnectionString;
        }

        protected override DbCommand GetCommand()
        {
            return new SQLiteCommand();
        }

        protected override DbConnection GetConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        protected override string GetConnectionString()
        {
            return _cs;
        }

        protected override DataAdapter GetDataAdapter(DbCommand cmd)
        {
            return new SQLiteDataAdapter(cmd as SQLiteCommand);
        }
    }
}
