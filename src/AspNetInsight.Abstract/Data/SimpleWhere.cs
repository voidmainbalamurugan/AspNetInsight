using System;
using System.Data;
using System.Data.Common;

namespace AspNetInsight.Data
{
    /// <summary>
    /// Represents the SQL Where clause, 
    /// could be used to build simple SQL where condition
    /// </summary>
    public abstract class SimpleWhere
    {
        public string Left { get; set; }
        public string Operator { get; set; }
        public DbType DataType { get; set; }
        public object RightValue { get; set; }
        public bool IsEnd { get; set; }
        public string NextOp { get; set; }
        public const string ParamPrefix = "@s";

        protected SimpleWhere(string left, string operand, DbType type, object right, bool end, string next = "")
        {
            Left = string.IsNullOrWhiteSpace(left) ? throw new ArgumentNullException(nameof(left))
                : left.Trim();
            Operator = string.IsNullOrWhiteSpace(operand) ? throw new ArgumentNullException(nameof(operand))
                : operand.Trim();

            DataType = type;

            RightValue = right?? throw new ArgumentNullException(nameof(right));
            IsEnd = end;
            if (!end)
                NextOp = string.IsNullOrWhiteSpace(next) ? throw new ArgumentNullException(nameof(next))
                : next.Trim();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {4}{2} {3}", Left, Operator, Left, IsEnd ? "" : NextOp, ParamPrefix);
        }

        public abstract DbParameter GetParameter();
    }
}
