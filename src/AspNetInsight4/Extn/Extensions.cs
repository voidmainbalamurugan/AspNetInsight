using AspNetInsight.Data;
using AspNetInsight4.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetInsight4
{
    /// <summary>
    /// Common extensions used across AspNetInsight4
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// DateTime converter
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static DateTime AspNetInsightNow(this DateTime @this)
        {
            return @this.ToUniversalTime();
        }

        #region SQLite helper methods

        internal static ColumnNameWithValue GetFromValue(this object @this, string name, DbType type)
        {
            return new SQLiteColumnWithValue(name, @this, type);
        }

        internal static SimpleWhere GetCondition(this object @this, string name, DbType type, string operand)
        {
            return new SQLiteWhere(name, operand, type, @this, true);
        }

        internal static BuildWhere Build(this object @this, string name, DbType type, string operand)
        {
            var sw = new SQLiteWhere(name, operand, type, @this, true);
            return new BuildWhere(sw);
        }

        #endregion

        /// <summary>
        /// Get resource data from executing assembly by resource name
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        internal static string GetFromResouce(this string resourceName)
        {
            string rtn = string.Empty;
            using (var st = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] byt = new byte[st.Length];
                st.Read(byt, 0, (int)st.Length);
                rtn = Encoding.UTF8.GetString(byt);
            }
            return rtn;
        }

        internal static TimeSpan GetTimeSpan(this object value)
        {
            return TimeSpan.FromTicks(Convert.ToInt64(value));
        }
        
    }
}
