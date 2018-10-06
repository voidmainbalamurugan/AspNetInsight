using System;
using System.Data;
using System.Data.Common;

namespace AspNetInsight.Data
{
    /// <summary>
    /// Represents Column name used in SQL query 'where' clause.
    /// </summary>
    public abstract class ColumnNameWithValue
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public DbType dBType { get; set; }
        public const string ParamPrefix4Update = "@u";
        public const string ParamPrefix4Insert = "@i";

        public ColumnNameWithValue(string name, object value, DbType type)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name))
                : name.Trim();
            Value = value ?? throw new ArgumentNullException(nameof(value));

            dBType = type;
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}{0}", Name, ParamPrefix4Update);
        }

        public string ValueString()
        {
            return ParamPrefix4Insert + Name;
        }

        public abstract DbParameter GetParameter(string prefix = ParamPrefix4Update);

    }
}