using System;
using System.Collections.Generic;
using AspNetInsight.Data;

namespace AspNetInsight4.SQLite
{
    /// <summary>
    /// Fluent interface that builds SimpleWhere clause.
    /// </summary>
    internal class BuildWhere
    {
        readonly List<SimpleWhere> _lst = new List<SimpleWhere>();
        SimpleWhere _recent { get; set; }
        const string _and = " AND ";
        const string _or = " OR ";

        public BuildWhere(SimpleWhere first)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            _lst.Add(first);
            _recent = first;
        }
               
        public BuildWhere And(SimpleWhere where)
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));
            lock (_lst)
            {
                _lst.Add(where);
                lock (_recent)
                {
                    _recent.IsEnd = false;
                    _recent.NextOp = _and;
                    _recent = where;
                }
            }
            return this;
        }

        public BuildWhere Or(SimpleWhere where)
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));
            lock (_lst)
            {
                _lst.Add(where);
                lock (_recent)
                {
                    _recent.IsEnd = false;
                    _recent.NextOp = _or;
                    _recent = where;
                }
            }
            return this;
        }

        public List<SimpleWhere> Return
        {
            get
            {
                return _lst;
            }
        }
        
    }
}
