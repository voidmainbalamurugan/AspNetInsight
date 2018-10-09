using AspNetInsight.Dto;
using AspNetInsight.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Data.SQLite.Generic;
using AspNetInsight.Data;
using System.Data;
using System.Collections.ObjectModel;
using AspNetInsight4.SQLite;
using AspNetInsight;

namespace AspNetInsight4.Repo
{
    /// <summary>
    /// Implements IAppRepo and represents App details table in SQLite data store
    /// </summary>
    public class SQLiteAppRepo : InsightTable<App>, IAppRepo
    {
        public override string EntityName { get { return "AppDetails"; } }

        public override ReadOnlyCollection<string> Columns => _columns.AsReadOnly();

        readonly List<string> _columns = new List<string>()
        {
            nameof(App.Id), nameof(App.AppId), nameof(App.AppName),
            nameof(App.MachineName), nameof(App.Url)
        };

        public override int CreationOrder => 0;
        public override string TableDefinitionSQL
        {
            get
            {
                var createScript = @"CREATE TABLE IF NOT EXISTS [AppDetails] (
                                  [Id] integer NOT NULL PRIMARY KEY AUTOINCREMENT
                                , [AppId] TEXT NOT NULL
                                , [AppName]
                                            TEXT NOT NULL
                                , [MachineName]
                                            TEXT NOT NULL
                                , [Url]
                                            TEXT NOT NULL
                                );";
                return createScript;
            }
        }

        readonly ICommandTextBuilder _commandHelper = new SQLiteCommon();
        protected override ICommandTextBuilder CommandHelper => _commandHelper;
        
        public App GetApp(long id)
        {
            var app = GetOnly(id.Build(nameof(App.Id), DbType.Int64, "=")
                .Return);
            if (app.Any())
                return app.FirstOrDefault();

            return null;
        }

        public App GetAppById(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentNullException(nameof(appId));

            var app = GetOnly(appId.Build(nameof(App.AppId), DbType.String, "=")
                .Return);
            if (app.Any())
                return app.FirstOrDefault();

            return null;

        }

        public IEnumerable<App> GetAppByMachineName(string machineName)
        {
            if (string.IsNullOrWhiteSpace(machineName))
                throw new ArgumentNullException(nameof(machineName));

            var app = GetOnly(machineName.Build(nameof(App.MachineName), DbType.String, "=")
                .Return);
            if (app.Any())
                return app.ToList();

            return default(IEnumerable<App>);
        }

        public IEnumerable<App> GetAppByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            var app = GetOnly(url.Build(nameof(App.Url), DbType.String, "=")
                .Return);
            if (app.Any())
                return app.ToList();

            return default(IEnumerable<App>);
        }

        public App New(App toAdd)
        {
            if (toAdd == default(App))
                throw new ArgumentNullException(nameof(toAdd));

            var app = Exists(toAdd);
            if (app == null)
            {
                app = toAdd.DeepCopy();
                var l = Insert(app);
                app.Id = Convert.ToInt64(l);
            }
            return app;
        }

        public App Exists(App app)
        {
            var data = GetOnly(app.AppId.Build(nameof(App.AppId), DbType.String, "=")
                .And(app.AppName.GetCondition(nameof(App.AppName), DbType.String, "="))
                .And(app.MachineName.GetCondition(nameof(App.MachineName), DbType.String, "="))
                .And(app.Url.GetCondition(nameof(App.Url), DbType.String, "="))
                .Return);
            return data.Any() ? data.FirstOrDefault() : null;
        }

        protected override Database GetDb()
        {
            return new SQLiteDatabase();
        }

        protected override App GetFromRow(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            return new App()
            {
                Id = Convert.ToInt64(row[nameof(App.Id)]),
                AppId = Convert.ToString(row[nameof(App.AppId)]),
                AppName = Convert.ToString(row[nameof(App.AppName)]),
                MachineName = Convert.ToString(row[nameof(App.MachineName)]),
                Url = Convert.ToString(row[nameof(App.Url)]),
            };
        }

        protected override List<ColumnNameWithValue> GetValues(App entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new List<ColumnNameWithValue>()
            {
                entity.AppId.GetFromValue(nameof(App.AppId), DbType.String),
                entity.AppName.GetFromValue(nameof(App.AppName), DbType.String),
                entity.MachineName.GetFromValue(nameof(App.MachineName), DbType.String),
                entity.Url.GetFromValue(nameof(App.Url), DbType.String)
            };
        }
    }
}
