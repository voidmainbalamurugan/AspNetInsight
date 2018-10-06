using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace AspNetInsight.Data
{
    /// <summary>
    /// Represents the Table in common Data store
    /// </summary>
    /// <typeparam name="TEntity">DTO is an object that represents table.</typeparam>
    public abstract class InsightTable
        <TEntity>
        where TEntity : class, new()
    {
        public abstract string EntityName { get; }
        public abstract ReadOnlyCollection<string> Columns { get; }
        protected abstract TEntity GetFromRow(DataRow row);
        protected abstract List<ColumnNameWithValue> GetValues(TEntity entity);
        protected abstract string TableDefinitionSQL { get; }
        protected abstract int CreationOrder { get; }
        protected abstract ICommandTextBuilder CommandHelper { get; }
        protected abstract Database GetDb();

        /// <summary>
        /// Returns all records as entities for given table
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetAll()
        {
            return GetWithFilters(null);
        }

        /// <summary>
        /// Returns all records as entities for given filters
        /// </summary>
        /// <param name="filters">used as where clause in SQL query</param>
        /// <returns></returns>
        public List<TEntity> GetOnly(List<SimpleWhere> filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            return GetWithFilters(filters);
        }

        /// <summary>
        /// Updates the specific records(with filters) for given table, 
        /// </summary>
        /// <param name="values">values to be updated</param>
        /// <param name="filters">used as where clause in SQL query</param>
        public void Update(List<ColumnNameWithValue> values, List<SimpleWhere> filters = null)
        {
            using (var cmd = CommandHelper.GetUpdate(EntityName, values, filters))
            {
                using (var db = GetDb())
                {
                    db.ExecuteNonQuery(cmd);
                }
            }
        }

        /// <summary>
        /// Updates the specific records(with filters) for given table, and return them as entities 
        /// </summary>
        /// <param name="values">values to be updated</param>
        /// <param name="filters">used as where clause in SQL query</param>
        public List<TEntity> UpdateAndGet(List<ColumnNameWithValue> values, List<SimpleWhere> filters = null)
        {
            var cmd = CommandHelper.GetUpdate(EntityName, values, filters);
            var scmd = CommandHelper.GetSelect(EntityName, Columns.ToList(), filters);
            cmd.CommandText += ";" + scmd.CommandText;
            foreach (var sp in scmd.Parameters)
                cmd.Parameters.Add(sp);
            return ExecuteAndGetData(cmd);    
        }

        /// <summary>
        /// Insert an entity into given table
        /// </summary>
        /// <param name="entity">entity to be added</param>
        /// <returns></returns>
        public object Insert(TEntity entity)
        {
            var values = GetValues(entity);
            object rtn = null;
            using (var cmd = CommandHelper.GetInsert(EntityName, values))
            {
                using (var db = GetDb())
                {
                    rtn = db.ExecuteScalar(cmd);
                }
            }
            return rtn;
        }

        /// <summary>
        /// Delete the records for the given filter condition
        /// </summary>
        /// <param name="filters">used as where clause in SQL query</param>
        public void GetDelete(List<SimpleWhere> filters)
        {
            using (var cmd = CommandHelper.GetDetete(EntityName, filters))
            {
                using (var db = GetDb())
                {
                    db.ExecuteNonQuery(cmd);
                }
            }
        }

        List<TEntity> GetWithFilters(List<SimpleWhere> filters = null)
        {
            var cmd = CommandHelper.GetSelect(EntityName, Columns.ToList(), filters);

            return ExecuteAndGetData(cmd);
        }

        List<TEntity> ExecuteAndGetData(DbCommand cmd)
        {
            DataTable dt = null;
            using (cmd)
            {
                using (var db = GetDb())
                {
                    var ds = db.ExecuteQuery(cmd);
                    dt = ds.Tables.Count > 0 ? ds.Tables[0] : null;
                }
            }
            if (dt == null)
                return null;
            List<TEntity> lst = new List<TEntity>();
            foreach (DataRow r in dt.Rows)
            {
                lst.Add(GetFromRow(r));
            }
            return lst;
        }
    }
}