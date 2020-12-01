using Maincotech.Utilities;
using System;
using System.Data;
using System.Data.Common;

namespace Maincotech.Data
{
    public sealed class DbHelper : DisposableObject
    {
        private bool _isDisposed;

        public DbHelper(string providerName, string connectionString)
        {
            ProviderName = providerName;
            ConnectionString = connectionString;
            Initialize();
        }

        /// <summary>
        /// get current connection's DbConnection object
        /// </summary>
        public DbConnection Connection { get; private set; }

        public string ConnectionString { get; }

        /// <summary>
        /// get current connection's db provider factory object
        /// </summary>
        public DbProviderFactory ProviderFactory { get; private set; }

        public string ProviderName { get; }

        #region Static Methods

        public static DbConnection CreateConnection(string providerName, string connectionString)
        {
            var dbfactory = DbProviderFactories.GetFactory(providerName);
            var dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = connectionString;
            return dbconn;
        }

        #endregion Static Methods

        #region Private Methods

        private void Initialize()
        {
            ParameterChecker.ArgumentNotNullOrEmptyString(ProviderName, "ProviderName");
            ValidateProviderName(ProviderName);

            ParameterChecker.ArgumentNotNullOrEmptyString(ConnectionString, "ConnectionString");

            ProviderFactory = DbProviderFactories.GetFactory(ProviderName);

            Connection = ProviderFactory.CreateConnection();
            Connection.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// check if the specific provider is existed or not
        /// </summary>
        /// <param name="providerName"></param>
        private void ValidateProviderName(string providerName)
        {
            var findResult = default(DataRow);
            foreach (DataRow item in DbProviderFactories.GetFactoryClasses().Rows)
            {
                if (item["InvariantName"].ToString().Equals(providerName, StringComparison.OrdinalIgnoreCase))
                {
                    findResult = item;
                    break;
                }
            }
            ParameterChecker.Against<ArgumentException>(findResult == null, string.Format("Provider name {0} is not existed", providerName));
        }

        #endregion Private Methods

        #region [====DisposableObject====]

        /// <summary>
        /// a delegate dispose method handle manual invoke or inner invoke
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                if (disposing)
                {
                    // Dispose of any managed resources of the derived class here.
                    Connection.Dispose();
                    // Call the base class implementation.
                }
                // Dispose of any unmanaged resources of the derived class here.
            }
        }

        #endregion [====DisposableObject====]

        public void AddInParameter(DbCommand cmd, string parameterName, DbType dbType, object value)
        {
            var dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            if (value == null)
            {
                dbParameter.Value = DBNull.Value;
            }
            else if (value.IsNullable())
            {
                dbParameter.Value = value.GetNullaleTypeValue();
            }
            else
            {
                dbParameter.Value = value;
            }
            dbParameter.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddInParameter(DbCommand cmd, string parameterName, object value)
        {
            var dbParameter = cmd.CreateParameter();
            dbParameter.ParameterName = parameterName;
            if (value == null)
            {
                dbParameter.Value = DBNull.Value;
            }
            else if (value.IsNullable())
            {
                dbParameter.Value = value.GetNullaleTypeValue();
            }
            else
            {
                dbParameter.Value = value;
            }
            dbParameter.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddOutParameter(DbCommand cmd, string parameterName, DbType dbType, int size)
        {
            var dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Size = size;
            dbParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddParameterCollection(DbCommand cmd, DbParameterCollection dbParameterCollection)
        {
            foreach (DbParameter dbParameter in dbParameterCollection)
            {
                cmd.Parameters.Add(dbParameter);
            }
        }

        public void AddReturnParameter(DbCommand cmd, string parameterName, DbType dbType)
        {
            var dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(dbParameter);
        }

        public DbTransaction BeingTransaction()
        {
            OpenConnection();
            return Connection.BeginTransaction();
        }

        /// <summary>
        /// Close the connection if the connection is opened
        /// </summary>
        public void CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
                Connection.Close();
        }

        public DataSet ExecuteDataSet(DbCommand cmd)
        {
            var dbfactory = DbProviderFactories.GetFactory(ProviderName);
            using (var dbDataAdapter = dbfactory.CreateDataAdapter())
            {
                dbDataAdapter.SelectCommand = cmd;
                var ds = new DataSet();
                dbDataAdapter.Fill(ds);
                return ds;
            }
        }

        public DataTable ExecuteDataTable(DbCommand cmd)
        {
            var dbfactory = DbProviderFactories.GetFactory(ProviderName);
            using (var dbDataAdapter = dbfactory.CreateDataAdapter())
            {
                dbDataAdapter.SelectCommand = cmd;
                var dataTable = new DataTable();
                dbDataAdapter.Fill(dataTable);

                return dataTable;
            }
        }

        public int ExecuteNonQuery(DbCommand cmd)
        {
            EnsureConnectionState(cmd.Connection);
            var ret = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return ret;
        }

        public DbDataReader ExecuteReader(DbCommand cmd)
        {
            EnsureConnectionState(cmd.Connection);
            var reader = cmd.ExecuteReader();
            return reader;
        }

        public object ExecuteScalar(DbCommand cmd)
        {
            EnsureConnectionState(cmd.Connection);
            var ret = cmd.ExecuteScalar();
            cmd.Connection.Close();
            return ret;
        }

        public DbParameter GetParameter(DbCommand cmd, string parameterName)
        {
            return cmd.Parameters[parameterName];
        }

        public DbCommand GetSqlStringCommond(string sqlQuery)
        {
            var dbCommand = Connection.CreateCommand();
            dbCommand.CommandText = sqlQuery;
            dbCommand.CommandType = CommandType.Text;
            return dbCommand;
        }

        public DbCommand GetStoredProcCommond(string storedProcedure)

        {
            var dbCommand = Connection.CreateCommand();
            dbCommand.CommandText = storedProcedure;
            dbCommand.CommandType = CommandType.StoredProcedure;
            return dbCommand;
        }

        /// <summary>
        /// Open the connection if the connection is closed
        /// </summary>
        public void OpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        private void EnsureConnectionState(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
    }
}