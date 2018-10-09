using AspNetInsight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AspNetInsight.Data
{
    /// <summary>
    /// Contains, abstract functionalities used by common Data store
    /// </summary>
    public abstract class Database : IDisposable
    {
        const string _tableDefSqlPropName = nameof(InsightTable<object>.TableDefinitionSQL); //"TableDefinitionSQL";
        const string _orderPropName = nameof(InsightTable<object>.CreationOrder); // "CreationOrder";

        DbConnection _connection { get; set; }
        Type _dtype { get; set; }
        public static bool DBCreated { get; private set; }

        protected abstract string GetConnectionString();
        protected abstract DbConnection GetConnection(string connectionString);
        protected abstract DbCommand GetCommand();
        protected abstract DataAdapter GetDataAdapter(DbCommand cmd);
        protected abstract void CreateDb();

        protected Database(Type dType = null)
        {
            _dtype = dType;
            if (!DBCreated)
            {
                CreateDb();
                CreateTables();
                DBCreated = true;

            }
            _connection = GetConnection(GetConnectionString());
        }

        /// <summary>
        /// Returns active connection with opened status
        /// </summary>
        /// <returns></returns>
        public DbConnection GetOpenConnection()
        {
            var con = GetConnection(GetConnectionString());
            con.Open();
            return con;
        }

        /// <summary>
        /// Creates table definition in given data store on startup, 
        /// definitions are read from InsightTable instances available in calling assembly.
        /// </summary>
        protected void CreateTables()
        {
            var tables = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.BaseType?.Name == typeof(InsightTable<>).Name);

            if(!tables.Any() && _dtype != null)
                tables = _dtype.Assembly
                    .GetTypes()
                    .Where(t => t.BaseType?.Name == typeof(InsightTable<>).Name);
            
            if (tables.Any())
                CreateTable(tables);
        }

        static IEnumerable<string> GetCommandText(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            var lst = new List<Tuple<int, string>>();
            object instance, value;
            string txt = string.Empty;
            int order;
            foreach (var t in types)
            {
                if (t == null)
                    continue;

                instance = Activator.CreateInstance(t);
                value = instance.GetType()
                            .GetProperty(_tableDefSqlPropName,
                                BindingFlags.Public | BindingFlags.Instance)
                                .GetValue(instance, null);
                if (value == null)
                    continue;

                txt = Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(txt))
                    continue;
                order = Convert.ToInt32(instance.GetType()
                            .GetProperty(_orderPropName,
                                BindingFlags.Public | BindingFlags.Instance)
                                .GetValue(instance, null));
                lst.Add(new Tuple<int, string>(order, txt));
            }

            return lst.OrderBy(t => t.Item1).ToList().ConvertAll(t => t.Item2);
        }

        protected void CreateTable(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            using (var con = GetOpenConnection())
            {
                var txtAll = GetCommandText(types).ToList();
                using (var trans = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = GetCommand())
                        {
                            cmd.Connection = con;
                            cmd.Transaction = trans;
                            cmd.CommandText = string.Join("; ", txtAll);
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                        }
                    }
                    catch { trans.Rollback(); }

                }
            }
        }

        /// <summary>
        /// Executes the given command and returns the result as Dataset.
        /// </summary>
        /// <param name="command">DbCommand to be executed for results</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var data = new DataSet();
            command.Connection = _connection;
            GetDataAdapter(command).Fill(data);

            return data;
        }

        /// <summary>
        /// Executes the given command.
        /// </summary>
        /// <param name="command">DbCommand to be executed</param>
        public void ExecuteNonQuery(DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            command.Connection = _connection;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes the given command and return the result.
        /// </summary>
        /// <param name="command">DbCommand to be executed</param>
        /// <returns></returns>
        public object ExecuteScalar(DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            command.Connection = _connection;
            return command.ExecuteScalar();
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }
    }
}