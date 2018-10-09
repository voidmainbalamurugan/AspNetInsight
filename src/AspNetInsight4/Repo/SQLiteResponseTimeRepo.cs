using AspNetInsight;
using AspNetInsight.Data;
using AspNetInsight.Dto;
using AspNetInsight.Repo;
using AspNetInsight4.SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace AspNetInsight4.Repo
{
    /// <summary>
    /// Implements IResponseRepo and represents App's ResponseTime table in SQLite data store
    /// </summary>
    public class SQLiteResponseTimeRepo : InsightTable<AppResponseTime>, IResponseRepo<AppResponseTime>
    {
        public override string EntityName => "AppResponseTime";

        public override ReadOnlyCollection<string> Columns => _columns.AsReadOnly();

        private readonly List<string> _columns = new List<string>()
        {
            nameof(AppResponseTime.Id), nameof(AppResponseTime.AppId), nameof(AppResponseTime.Recent),
            nameof(AppResponseTime.Min), nameof(AppResponseTime.Avg), nameof(AppResponseTime.Max),
            nameof(AppResponseTime.Total), nameof(AppResponseTime.Slice), nameof(AppResponseTime.ModifiedDate)
        };

        public override int CreationOrder => 1;
        public override string TableDefinitionSQL
            => @"CREATE TABLE IF NOT EXISTS [AppResponseTime] (
              [Id] integer NOT NULL PRIMARY KEY AUTOINCREMENT
            , [Recent] double NOT NULL
            , [Min] double NOT NULL
            , [Avg] double NOT NULL
            , [Max] double NOT NULL
            , [Total] bigint  NOT NULL
            , [AppId] bigint  NOT NULL
            , [Slice] text NOT NULL
            , [ModifiedDate] datetime NOT NULL
            , FOREIGN KEY ([AppId]) REFERENCES [AppDetails] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
            );
            CREATE UNIQUE INDEX IF NOT EXISTS [idx_app_id] ON [AppResponseTime] ([AppId] ASC);
            CREATE TRIGGER IF NOT EXISTS update_response_time  
             BEFORE UPDATE ON AppResponseTime 
            BEGIN 
	            UPDATE AppResponseTime  
    	            SET Recent = new.recent 
    	            ,Min = (SELECT CASE WHEN old.Min > new.Recent THEN new.Recent ELSE old.Min END) 
    	            ,Max = (SELECT CASE WHEN old.Max < new.Recent THEN new.Recent ELSE old.Max END)  
    	            ,Avg = ((old.Total*old.Avg)+new.Recent)/(old.Total+1)  
    	            ,Total = old.Total+1
		            ,ModifiedDate = (SELECT DATETIME('now', 'utc'))
	            WHERE Id = old.Id and AppId = old.AppId;
            END;";

        readonly ICommandTextBuilder _commandHelper = new SQLiteCommon();
        protected override ICommandTextBuilder CommandHelper => _commandHelper;
        SQLiteAppRepo _appRepo { get; set; }

        public SQLiteResponseTimeRepo()
            :this(null)
        { }

        public SQLiteResponseTimeRepo(SQLiteAppRepo appRepo)
        {
            _appRepo = appRepo ?? new SQLiteAppRepo();
        }

        public AppResponseTime GetAppData(long id)
        {
            var data = GetOnly(id.Build(nameof(AppResponseTime.Id), DbType.Int64, "=")
                 .Return);
            if (data.Any())
                return data.FirstOrDefault();

            return null;
        }

        public AppResponseTime GetAppDataById(long appId)
        {
            var data = GetOnly(appId.Build(nameof(AppResponseTime.AppId), DbType.Int64, "=")
                 .Return);
            if (data.Any())
                return data.FirstOrDefault();

            return null;
        }

        public AppResponseTime UpdateRecentByAppId(long appId, double recent)
        {
            var app = _appRepo.GetApp(appId);
            if (app == null)
                throw new ArgumentException(nameof(appId));

            var data = GetAppDataById(appId);
            if (data != null)
            {
                data.Recent = recent;
                var rslt = Update(data);
                data = rslt.Any() ? rslt.FirstOrDefault() : null;
            }
            else
            {
                data = new AppResponseTime()
                {
                    AppId = appId,
                    Avg = recent,
                    Min = recent,
                    Max = recent,
                    Recent = recent,
                    Slice = TimeSlice.Milliseconds,
                    Total = 1,
                    ModifiedDate = DateTime.Now.Date.AspNetInsightNow()
                };
                data.Id = Convert.ToInt64(Insert(data));
            }

            return data;
        }

        List<AppResponseTime> Update(AppResponseTime data)
        {
            return UpdateAndGet(new List<ColumnNameWithValue>()
                    { data.Recent.GetFromValue(nameof(AppResponseTime.Recent), DbType.Double)},
                      data.AppId.Build(nameof(AppResponseTime.AppId), DbType.Int64, "=")
                    .And(data.Id.GetCondition(nameof(AppResponseTime.Id), DbType.Int64, "="))
                    .Return);
        }

        public AppResponseTime AddAppData(AppResponseTime appData)
        {
            if (appData == null)
                throw new ArgumentNullException(nameof(appData));

            var data = GetAppDataById(appData.AppId);
            if (data == null)
            {
                data = appData.DeepCopy();
                var l = Insert(data);
                data.Id = Convert.ToInt64(l);
            }
            else
            {
                data.Recent = appData.Recent;
                var eData = Update(data);
                if (eData.Any())
                    data = eData.FirstOrDefault();
            }
            return data;
        }

        protected override Database GetDb()
        {
            return new SQLiteDatabase();
        }

        protected override AppResponseTime GetFromRow(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            return new AppResponseTime()
            {
                Id = Convert.ToInt64(row[nameof(AppResponseTime.Id)]),
                AppId = Convert.ToInt64(row[nameof(AppResponseTime.AppId)]),
                Avg = Convert.ToDouble(row[nameof(AppResponseTime.Avg)]),
                Min = Convert.ToDouble(row[nameof(AppResponseTime.Min)]),
                Max = Convert.ToDouble(row[nameof(AppResponseTime.Max)]),
                Recent = Convert.ToDouble(row[nameof(AppResponseTime.Recent)]),
                Total = Convert.ToInt64(row[nameof(AppResponseTime.Total)]),
                Slice = (TimeSlice)Enum.Parse(typeof(TimeSlice), row[nameof(AppResponseTime.Slice)].ToString()),
                ModifiedDate = Convert.ToDateTime(row[nameof(AppResponseTime.ModifiedDate)])
            };
        }

        protected override List<ColumnNameWithValue> GetValues(AppResponseTime entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new List<ColumnNameWithValue>()
            {
                entity.AppId.GetFromValue(nameof(AppResponseTime.AppId), DbType.Int64),
                entity.Avg.GetFromValue(nameof(AppResponseTime.Avg), DbType.Double),
                entity.Min.GetFromValue(nameof(AppResponseTime.Min), DbType.Double),
                entity.Recent.GetFromValue(nameof(AppResponseTime.Recent), DbType.Double),
                entity.Max.GetFromValue(nameof(AppResponseTime.Max), DbType.Double),
                entity.Total.GetFromValue(nameof(AppResponseTime.Total), DbType.Int64),
                entity.Slice.ToString().GetFromValue(nameof(AppResponseTime.Slice), DbType.String),
                entity.ModifiedDate.GetFromValue(nameof(AppResponseTime.ModifiedDate), DbType.DateTime)
            };
        }
    }
}
