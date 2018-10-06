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
    /// Implements ILogRepo and represents ResponseLog insight table for SQLite data store
    /// </summary>
    public class LogRepo : InsightTable<ResponseLog>, ILogRepo<ResponseLog>
    {
        public override string EntityName => "ResponseLog";

        public override ReadOnlyCollection<string> Columns => _columns.AsReadOnly();

        readonly List<string> _columns = new List<string>()
        {
            nameof(ResponseLog.Id), nameof(ResponseLog.AppId), nameof(ResponseLog.MachineName),
            nameof(ResponseLog.RawUrl), nameof(ResponseLog.Url), nameof(ResponseLog.ResponseTime),
            nameof(ResponseLog.HanlderExeTime), nameof(ResponseLog.TimeScale), nameof(ResponseLog.ByteSent),
            nameof(ResponseLog.Scale), nameof(ResponseLog.CreatedOn)
        };

        protected override int CreationOrder => 2;
        protected override string TableDefinitionSQL 
            => @"CREATE TABLE IF NOT EXISTS [ResponseLog] (
                  [Id] integer NOT NULL PRIMARY KEY AUTOINCREMENT
                , [AppId] bigint  NOT NULL
                , [MachineName] text NOT NULL
                , [Url] text NOT NULL
                , [RawUrl] text NOT NULL
                , [TimeScale] text NOT NULL
                , [Scale] text NOT NULL
                , [ResponseTime] double NOT NULL
                , [HanlderExeTime] double NOT NULL
                , [ByteSent] double NOT NULL
                , [CreatedOn] datetime NOT NULL
                , FOREIGN KEY ([AppId]) REFERENCES [AppDetails] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
                );";

        ICommandTextBuilder _commandHelper = new SQLiteCommon();
        protected override ICommandTextBuilder CommandHelper => _commandHelper;

        public IEnumerable<ResponseLog> GetByAppId(long appId)
        {
            var data = GetOnly(appId.Build(nameof(ResponseLog.AppId), DbType.Int64, "=")
                .Return);
            if (data.Any())
                return data.ToList();

            return null;
        }

        public IEnumerable<ResponseLog> GetByAppId(long appId, DateTime from, DateTime to)
        {
            var data = GetOnly(appId.Build(nameof(ResponseLog.AppId), DbType.Int64, "=")
                .And(from.GetCondition(nameof(ResponseLog.CreatedOn), DbType.DateTime, ">="))
                .And(to.GetCondition(nameof(ResponseLog.CreatedOn), DbType.DateTime, "<="))
                .Return);
            if (data.Any())
                return data.ToList();

            return null;
        }

        public void Log(ResponseLog dataToLog)
        {
            if (dataToLog == null)
                throw new ArgumentNullException(nameof(dataToLog));
            if (dataToLog.Id > 0)
                throw new ArgumentException(nameof(dataToLog));

            Insert(dataToLog);
        }

        protected override Database GetDb()
        {
            return new SQLiteDatabase();
        }

        protected override ResponseLog GetFromRow(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            return new ResponseLog()
            {
                Id = Convert.ToInt64(row[nameof(ResponseLog.Id)]),
                AppId = Convert.ToInt64(row[nameof(ResponseLog.AppId)]),
                ResponseTime = Convert.ToDouble(row[nameof(ResponseLog.ResponseTime)]),
                HanlderExeTime = Convert.ToDouble(row[nameof(ResponseLog.HanlderExeTime)]),
                ByteSent = Convert.ToDouble(row[nameof(ResponseLog.ByteSent)]),
                RawUrl = Convert.ToString(row[nameof(ResponseLog.RawUrl)]),
                Url = Convert.ToString(row[nameof(ResponseLog.Url)]),
                MachineName = Convert.ToString(row[nameof(ResponseLog.MachineName)]),
                TimeScale = (TimeSlice)Enum.Parse(typeof(TimeSlice), row[nameof(ResponseLog.TimeScale)].ToString()),
                CreatedOn = Convert.ToDateTime(row[nameof(ResponseLog.CreatedOn)]),
                Scale = (Size)Enum.Parse(typeof(Size), row[nameof(ResponseLog.Scale)].ToString())
            };
        }

        protected override List<ColumnNameWithValue> GetValues(ResponseLog entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new List<ColumnNameWithValue>()
            {
                //entity.Id.GetFromValue(nameof(ResponseLog.Id), DbType.Int64),
                entity.AppId.GetFromValue(nameof(ResponseLog.AppId), DbType.Int64),
                entity.ResponseTime.GetFromValue(nameof(ResponseLog.ResponseTime), DbType.Double),
                entity.HanlderExeTime.GetFromValue(nameof(ResponseLog.HanlderExeTime), DbType.Double),
                entity.ByteSent.GetFromValue(nameof(ResponseLog.ByteSent), DbType.Double),
                entity.RawUrl.GetFromValue(nameof(ResponseLog.RawUrl), DbType.String),
                entity.Url.GetFromValue(nameof(ResponseLog.Url), DbType.String),
                entity.MachineName.GetFromValue(nameof(ResponseLog.MachineName), DbType.String),
                entity.TimeScale.ToString().GetFromValue(nameof(ResponseLog.TimeScale), DbType.String),
                entity.Scale.ToString().GetFromValue(nameof(ResponseLog.Scale), DbType.String),
                entity.CreatedOn.GetFromValue(nameof(ResponseLog.CreatedOn), DbType.DateTime)
            };
        }
    }
}