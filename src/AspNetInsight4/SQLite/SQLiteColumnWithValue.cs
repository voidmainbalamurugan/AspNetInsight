using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using AspNetInsight.Data;

namespace AspNetInsight4.SQLite
{
    /// <summary>
    /// SQLite Column definitions 
    /// </summary>
    internal class SQLiteColumnWithValue : ColumnNameWithValue
    {
        public SQLiteColumnWithValue(string name, object value, DbType type) : base(name, value, type)
        {
        }

        public override DbParameter GetParameter(string prefix = ParamPrefix4Update)
        {
            return new SQLiteParameter(string.Format("{0}{1}", prefix,  Name), dBType) { Value = this.Value };
        }
    }
}
