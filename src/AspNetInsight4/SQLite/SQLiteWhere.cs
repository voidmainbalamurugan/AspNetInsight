using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using AspNetInsight.Data;

namespace AspNetInsight4.SQLite
{
    /// <summary>
    /// SimpleWhere for SQLite data store
    /// </summary>
    internal class SQLiteWhere : SimpleWhere
    {
        public SQLiteWhere(string left, string operand, DbType type, object right, bool end, string next = "") : base(left, operand, type, right, end, next)
        {
        }

        public override DbParameter GetParameter()
        {
            return new SQLiteParameter(string.Format("{1}{0}", Left, ParamPrefix), dBType) { Value = RightValue };
        }
        
    }
}
