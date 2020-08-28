using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Maincotech.Data
{
    public abstract class DataProviderBase : DisposableObject
    {
        protected DbHelper DbHelper;

        protected FilterTranslator FilterTranslator;

        protected SortTranslator SortTranslator;

        public DataProviderBase()
        {
            SortTranslator = new SortTranslator();
            FilterTranslator = new FilterTranslator();
        }

        public DataProviderBase(DbHelper dbHelper)
        {
            this.DbHelper = dbHelper;
            SortTranslator = new SortTranslator();
            FilterTranslator = new FilterTranslator();
        }

        public FilterGroup Filter { get; set; }

        public string InnerJoin { get; set; }

        public string SelectFields { get; set; }

        public SortGroup Sort { get; set; }

        public string TableName { get; set; }

        protected abstract PagingTranslater PagingTranslater { get; }

        public virtual List<object> GetDistinctValues(string columnName)
        {
            var result = new List<object>();
            using (var cmd = GetDistinctValuesCmd(columnName))
            {
                using (var reader = DbHelper.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        result.Add(reader[0]);
                    }
                }
            }
            return result;
        }

        public virtual long GetTotalCount()
        {
            using (var cmd = GetTotalCountCmd())
            {
                return (long)DbHelper.ExecuteScalar(cmd);
            }
        }

        protected DbCommand GetTotalCountCmd()
        {
            var commandText = "select count(*) as Total from [" + TableName + "] " + InnerJoin;
            FilterTranslator filterTranslator = null;
            if (Filter != null)
            {
                filterTranslator = new FilterTranslator(Filter);
                filterTranslator.Translate();
                commandText += " where " + filterTranslator.CommandText;
            }
            var cmd = DbHelper.GetSqlStringCommond(commandText);
            if (filterTranslator != null && filterTranslator.Parms.Count > 0)
            {
                foreach (var parameter in filterTranslator.Parms)
                {
                    DbHelper.AddInParameter(cmd, parameter.Name, parameter.Value);
                }
            }
            return cmd;
        }

        public virtual object GetMaxValue(string columnName)
        {
            using (var cmd = GetMaxValueCmd(columnName))
            {
                return DbHelper.ExecuteScalar(cmd);
            }
        }

        public virtual object GetMinValue(string columnName)
        {
            using (var cmd = GetMinValueCmd(columnName))
            {
                return DbHelper.ExecuteScalar(cmd);
            }
        }

        protected DbCommand GetDistinctValuesCmd(string columnName)
        {
            var commandText = string.Format("SELECT DISTINCT({0}) as Total from [{1}]{2}", columnName, TableName, InnerJoin);
            FilterTranslator filterTranslator = null;
            if (Filter != null)
            {
                var hasCurrentColumnFilter = false;
                hasCurrentColumnFilter = Filter.Name.EqualsIgnoreCase(columnName);
                if (!hasCurrentColumnFilter)
                {
                    if (Filter.Groups != null && Filter.Groups.Count > 0)
                    {
                        hasCurrentColumnFilter = Filter.Groups.Any(group => group.Name.EqualsIgnoreCase(columnName));
                    }
                }
                FilterGroup tempFilter = null;

                if (hasCurrentColumnFilter)
                {
                    if (!Filter.Name.EqualsIgnoreCase(columnName))
                    {
                        tempFilter = Filter.DeepClone();
                        var currentColumnFilter = tempFilter.Groups.First(group => group.Name.EqualsIgnoreCase(columnName));
                        tempFilter.Groups.Remove(currentColumnFilter);
                    }
                }
                else
                {
                    tempFilter = Filter;
                }
                if (tempFilter != null)
                {
                    filterTranslator = new FilterTranslator(tempFilter);
                    filterTranslator.Translate();
                    commandText += " where " + filterTranslator.CommandText;
                }
            }

            var cmd = DbHelper.GetSqlStringCommond(commandText);
            if (filterTranslator != null && filterTranslator.Parms.Count > 0)
            {
                foreach (var parameter in filterTranslator.Parms)
                {
                    DbHelper.AddInParameter(cmd, parameter.Name, parameter.Value);
                }
            }
            return cmd;
        }

        protected DbCommand GetMaxValueCmd(string columnName)
        {
            var commandText = string.Format("select MAX({0}) as Total from [{1}]{2}", columnName, TableName, InnerJoin);

            FilterTranslator filterTranslator = null;
            if (Filter != null)
            {
                filterTranslator = new FilterTranslator(Filter);
                filterTranslator.Translate();
                commandText += " where " + filterTranslator.CommandText;
            }

            var cmd = DbHelper.GetSqlStringCommond(commandText);
            if (filterTranslator != null && filterTranslator.Parms.Count > 0)
            {
                foreach (var parameter in filterTranslator.Parms)
                {
                    DbHelper.AddInParameter(cmd, parameter.Name, parameter.Value);
                }
            }

            return cmd;
        }

        public virtual DataTable GetPagedData(Pagination pagination)
        {
            PagingTranslater.TableName = TableName;
            PagingTranslater.InnerJoin = InnerJoin;
            PagingTranslater.SelectFields = SelectFields;
            PagingTranslater.Pagination = pagination;
            PagingTranslater.Filter = Filter;
            PagingTranslater.Sort = Sort;
            PagingTranslater.Parms.Clear();
            if (pagination.Total.HasValue == false)
            {
                var countCmdText = PagingTranslater.GetTotalCommad();
                using (var cmd = DbHelper.GetSqlStringCommond(countCmdText))
                {
                    if (PagingTranslater.Parms.Count > 0)
                    {
                        foreach (var parameter in PagingTranslater.Parms)
                        {
                            DbHelper.AddInParameter(cmd, parameter.Name, parameter.Value);
                        }
                    }
                    pagination.Total = (int)DbHelper.ExecuteScalar(cmd);
                }
            }
            PagingTranslater.Parms.Clear();

            var getDataCmdText = PagingTranslater.GetDataCommand();
            using (var cmd = DbHelper.GetSqlStringCommond(getDataCmdText))
            {
                if (PagingTranslater.Parms.Count > 0)
                {
                    foreach (var parameter in PagingTranslater.Parms)
                    {
                        DbHelper.AddInParameter(cmd, parameter.Name, parameter.Value);
                    }
                }
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        protected DbCommand GetMinValueCmd(string columnName)
        {
            var commandText = string.Format("select min({0}) as Total from [{1}]{2}", columnName, TableName, InnerJoin);

            FilterTranslator filterTranslator = null;
            if (Filter != null)
            {
                filterTranslator = new FilterTranslator(Filter);
                filterTranslator.Translate();
                commandText += " where " + filterTranslator.CommandText;
            }

            var cmd = DbHelper.GetSqlStringCommond(commandText);
            if (filterTranslator != null && filterTranslator.Parms.Count > 0)
            {
                foreach (var parameter in filterTranslator.Parms)
                {
                    DbHelper.AddInParameter(cmd, parameter.Name, parameter.Value);
                }
            }

            return cmd;
        }

        protected override void Dispose(bool disposing)
        {
            if (DbHelper != null)
            {
                DbHelper.Dispose();
            }
        }
    }
}