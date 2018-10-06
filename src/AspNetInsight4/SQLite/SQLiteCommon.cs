﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SQLite;
using AspNetInsight.Data;
using System.Data.Common;

namespace AspNetInsight4.SQLite
{
    /// <summary>
    /// Common SQLite query builder
    /// for INSERT, SELECT, UPDATE and DELETE queries with where filter clause
    /// </summary>
    internal class SQLiteCommon : ICommandTextBuilder
    {
        /// <summary>
        /// Builds SELECT query for given table name, columns and filters
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public DbCommand GetSelect(string tableName, List<string> columns, List<SimpleWhere> filters = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (columns == null || !columns.Any())
                throw new ArgumentNullException(nameof(columns));

            var txt = string.Format("SELECT {0} FROM {1}", string.Join(", ", columns), tableName.Trim());

            var cmd = new SQLiteCommand(txt);
            if (filters != null && filters.Any())
            {
                cmd = AddWhere(cmd, filters);
            }
            return cmd;
        }

        /// <summary>
        /// Update query builder for given table name, columns and filter conditions
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public DbCommand GetUpdate(string tableName, List<ColumnNameWithValue> columns, List<SimpleWhere> filters)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (columns == null || !columns.Any())
                throw new ArgumentNullException(nameof(columns));

            var txt = string.Format("UPDATE {0} SET {1} ", tableName.Trim(),
                string.Join(", ", columns.ConvertAll(c => c.ToString())));

            var cmd = new SQLiteCommand(txt);
            cmd.Parameters.AddRange(columns.ConvertAll(c => c.GetParameter()).ToArray());
            if (filters != null && filters.Any())
            {
                cmd = AddWhere(cmd, filters);
            }
            return cmd;
        }

        /// <summary>
        /// Builds DELETE query for given table name and filter conditions
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public DbCommand GetDetete(string tableName, List<SimpleWhere> filters)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var txt = string.Format("DELETE FROM {0} ", tableName.Trim());
            var cmd = new SQLiteCommand(txt);
            if (filters != null && filters.Any())
            {
                cmd = AddWhere(cmd, filters);
            }
            return cmd;
        }

        /// <summary>
        /// Builds insert query for given table and column values
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DbCommand GetInsert(string tableName, List<ColumnNameWithValue> values)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (values == null || !values.Any())
                throw new ArgumentNullException(nameof(values));

            var txt = string.Format("INSERT INTO {0} ({1}) VALUES({2}) ", tableName.Trim(),
                string.Join(", ", values.ConvertAll(c => c.Name)),
                string.Join(", ", values.ConvertAll(c => c.ValueString())));

            var cmd = new SQLiteCommand(txt);
            cmd.Parameters.AddRange(values.ConvertAll(c => c.GetParameter(ColumnNameWithValue.ParamPrefix4Insert)).ToArray());

            cmd.CommandText += string.Format(GetSeqText, tableName) + ";";
            return cmd;
        }

        static SQLiteCommand AddWhere(SQLiteCommand cmd, List<SimpleWhere> filters)
        {
            var txt = " WHERE ";
            txt += string.Join(" ", filters.ConvertAll(f => f.ToString()));
            cmd.CommandText += txt;
            cmd.Parameters.AddRange(filters.ConvertAll(f => f.GetParameter()).ToArray());
            return cmd;
        }

        const string GetSeqText = ";SELECT seq FROM sqlite_sequence WHERE name = '{0}'";
    }
}
