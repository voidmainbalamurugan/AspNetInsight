using System.Collections.Generic;
using System.Data.Common;

namespace AspNetInsight.Data
{
    /// <summary>
    /// Common helper used to get SQL queries.
    /// </summary>
    public interface ICommandTextBuilder
    {
        DbCommand GetSelect(string tableName, List<string> columns, List<SimpleWhere> filters = null);
        DbCommand GetUpdate(string tableName, List<ColumnNameWithValue> columns, List<SimpleWhere> filters);
        DbCommand GetDetete(string tableName, List<SimpleWhere> filters);
        DbCommand GetInsert(string tableName, List<ColumnNameWithValue> values);
    }
}
