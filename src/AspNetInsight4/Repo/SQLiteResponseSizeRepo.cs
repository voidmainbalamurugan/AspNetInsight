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
    ///  Implements IResponseRepo and represents App's ResponseSize table in SQLite data store
    /// </summary>
    public class SQLiteResponseSizeRepo : InsightTable<AppResponseSize>, IResponseRepo<AppResponseSize>
    {
        public override string EntityName => "AppResponseSize";

        public override ReadOnlyCollection<string> Columns => _columns.AsReadOnly();

        readonly List<string> _columns = new List<string>()
        {
            nameof(AppResponseSize.Id), nameof(AppResponseSize.AppId), nameof(AppResponseSize.Recent),
            nameof(AppResponseSize.Min), nameof(AppResponseSize.Avg), nameof(AppResponseSize.Max),
            nameof(AppResponseSize.Total), nameof(AppResponseSize.Scale), nameof(AppResponseSize.ModifiedDate)
        };

        protected override int CreationOrder => 1;
        protected override string TableDefinitionSQL 
            => @"CREATE TABLE IF NOT EXISTS [AppResponseSize] (
                  [Id] integer NOT NULL PRIMARY KEY AUTOINCREMENT
                , [Recent] double NOT NULL
                , [Min] double NOT NULL
                , [Avg] double NOT NULL
                , [Max] double NOT NULL
                , [Total] bigint  NOT NULL
                , [AppId] bigint  NOT NULL
                , [Scale] text NOT NULL
                , [ModifiedDate] datetime NOT NULL
                , FOREIGN KEY ([AppId]) REFERENCES [AppDetails] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
                );
                CREATE UNIQUE INDEX IF NOT EXISTS [idx_app_id_size] ON [AppResponseSize] ([AppId] ASC);
                CREATE TRIGGER IF NOT EXISTS update_response_size 
                 BEFORE UPDATE ON [AppResponseSize] 
                BEGIN 
	                UPDATE [AppResponseSize]  
    	                SET Recent = new.recent 
    	                ,Min = (SELECT CASE WHEN old.Min > new.Recent THEN new.Recent ELSE old.Min END) 
    	                ,Max = (SELECT CASE WHEN old.Max < new.Recent THEN new.Recent ELSE old.Max END)  
    	                ,Avg = ((old.Total*old.Avg)+new.Recent)/(old.Total+1)  
    	                ,Total = old.Total+1
		                ,ModifiedDate = (SELECT DATETIME('now', 'utc'))
	                WHERE Id = old.Id and AppId = old.AppId;
                END;";

        ICommandTextBuilder _commandHelper = new SQLiteCommon();
        protected override ICommandTextBuilder CommandHelper => _commandHelper;
        SQLiteAppRepo _appRepo { get; set; }
        public SQLiteResponseSizeRepo()
            : this(null)
        { }

        public SQLiteResponseSizeRepo(SQLiteAppRepo appRepo)
        {
            _appRepo = appRepo ?? new SQLiteAppRepo();
        }

        public AppResponseSize GetAppData(long id)
        {
            var data = GetOnly(id.Build(nameof(AppResponseSize.Id), DbType.Int64, "=")
                 .Return);
            if (data.Any())
                return data.FirstOrDefault();

            return null;
        }

        public AppResponseSize GetAppDataById(long appId)
        {
            var data = GetOnly(appId.Build(nameof(AppResponseSize.AppId), DbType.Int64, "=")
                 .Return);
            if (data.Any())
                return data.FirstOrDefault();

            return null;
        }
        
        public AppResponseSize UpdateRecentByAppId(long appId, double recent)
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
                data = new AppResponseSize()
                {
                    AppId = appId,
                    Avg = recent,
                    Min = recent,
                    Max = recent,
                    Recent = recent,
                    Scale = Size.Byte,
                    Total = 1,
                    ModifiedDate = DateTime.Now.Date.AspNetInsightNow()
                };
                data.Id = Convert.ToInt64(Insert(data));
            }

            return data;
        }

        public AppResponseSize AddAppData(AppResponseSize appData)
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

        List<AppResponseSize> Update(AppResponseSize data)
        {
            return UpdateAndGet(new List<ColumnNameWithValue>()
                    { data.Recent.GetFromValue(nameof(AppResponseSize.Recent), DbType.Double)},
                      data.AppId.Build(nameof(AppResponseSize.AppId), DbType.Int64, "=")
                    .And(data.Id.GetCondition(nameof(AppResponseSize.Id), DbType.Int64, "="))
                    .Return);
        }

        protected override Database GetDb()
        {
            return new SQLiteDatabase();
        }
        
        protected override AppResponseSize GetFromRow(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            return new AppResponseSize()
            {
                Id = Convert.ToInt64(row[nameof(AppResponseSize.Id)]),
                AppId = Convert.ToInt64(row[nameof(AppResponseSize.AppId)]),
                Avg = Convert.ToDouble(row[nameof(AppResponseSize.Avg)]),
                Min = Convert.ToDouble(row[nameof(AppResponseSize.Min)]),
                Max = Convert.ToDouble(row[nameof(AppResponseSize.Max)]),
                Recent = Convert.ToDouble(row[nameof(AppResponseSize.Recent)]),
                Total = Convert.ToInt64(row[nameof(AppResponseSize.Total)]),
                Scale = (Size)Enum.Parse(typeof(Size), row[nameof(AppResponseSize.Scale)].ToString()),
                ModifiedDate = Convert.ToDateTime(row[nameof(AppResponseSize.ModifiedDate)])
            };
        }

        protected override List<ColumnNameWithValue> GetValues(AppResponseSize entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new List<ColumnNameWithValue>()
            {
                entity.AppId.GetFromValue(nameof(AppResponseSize.AppId), DbType.Int64),
                entity.Avg.GetFromValue(nameof(AppResponseSize.Avg), DbType.Double),
                entity.Min.GetFromValue(nameof(AppResponseSize.Min), DbType.Double),
                entity.Recent.GetFromValue(nameof(AppResponseSize.Recent), DbType.Double),
                entity.Max.GetFromValue(nameof(AppResponseSize.Max), DbType.Double),
                entity.Total.GetFromValue(nameof(AppResponseSize.Total), DbType.Int64),
                entity.Scale.ToString().GetFromValue(nameof(AppResponseSize.Scale), DbType.String),
                entity.ModifiedDate.GetFromValue(nameof(AppResponseSize.ModifiedDate), DbType.DateTime)
            };
        }
    }
}
