namespace IBatisNet.DataMapper
{
    using IBatisNet.Common;
    using IBatisNet.Common.Logging;
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Data;

    [Serializable]
    public class SqlMapSession : ISqlMapSession, IDalSession, IDisposable
    {
        private IDbConnection _connection;
        private bool _consistent;
        private IDataSource _dataSource;
        private bool _isTransactionOpen;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ISqlMapper _sqlMapper;
        private IDbTransaction _transaction;

        public SqlMapSession(ISqlMapper sqlMapper)
        {
            this._dataSource = sqlMapper.DataSource;
            this._sqlMapper = sqlMapper;
        }

        public void BeginTransaction()
        {
            this.BeginTransaction(this._dataSource.ConnectionString);
        }

        public void BeginTransaction(bool openConnection)
        {
            if (openConnection)
            {
                this.BeginTransaction();
            }
            else
            {
                if ((this._connection == null) || (this._connection.State != ConnectionState.Open))
                {
                    this.OpenConnection();
                }
                this._transaction = this._connection.BeginTransaction();
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("Begin Transaction.");
                }
                this._isTransactionOpen = true;
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            this.BeginTransaction(this._dataSource.ConnectionString, isolationLevel);
        }

        public void BeginTransaction(string connectionString)
        {
            if ((this._connection == null) || (this._connection.State != ConnectionState.Open))
            {
                this.OpenConnection(connectionString);
            }
            this._transaction = this._connection.BeginTransaction();
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Begin Transaction.");
            }
            this._isTransactionOpen = true;
        }

        public void BeginTransaction(bool openConnection, IsolationLevel isolationLevel)
        {
            this.BeginTransaction(this._dataSource.ConnectionString, openConnection, isolationLevel);
        }

        public void BeginTransaction(string connectionString, IsolationLevel isolationLevel)
        {
            if ((this._connection == null) || (this._connection.State != ConnectionState.Open))
            {
                this.OpenConnection(connectionString);
            }
            this._transaction = this._connection.BeginTransaction(isolationLevel);
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Begin Transaction.");
            }
            this._isTransactionOpen = true;
        }

        public void BeginTransaction(string connectionString, bool openConnection, IsolationLevel isolationLevel)
        {
            if (openConnection)
            {
                this.BeginTransaction(connectionString, isolationLevel);
            }
            else
            {
                if ((this._connection == null) || (this._connection.State != ConnectionState.Open))
                {
                    throw new DataMapperException("SqlMapSession could not invoke StartTransaction(). A Connection must be started. Call OpenConnection() first.");
                }
                this._transaction = this._connection.BeginTransaction(isolationLevel);
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("Begin Transaction.");
                }
                this._isTransactionOpen = true;
            }
        }

        public void CloseConnection()
        {
            if ((this._connection != null) && (this._connection.State != ConnectionState.Closed))
            {
                this._connection.Close();
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug(string.Format("Close Connection \"{0}\" to \"{1}\".", this._connection.GetHashCode().ToString(), this._dataSource.DbProvider.Description));
                }
                this._connection.Dispose();
            }
            this._connection = null;
        }

        public void CommitTransaction()
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Commit Transaction.");
            }
            this._transaction.Commit();
            this._transaction.Dispose();
            this._transaction = null;
            this._isTransactionOpen = false;
            if (this._connection.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }

        public void CommitTransaction(bool closeConnection)
        {
            if (closeConnection)
            {
                this.CommitTransaction();
            }
            else
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("Commit Transaction.");
                }
                this._transaction.Commit();
                this._transaction.Dispose();
                this._transaction = null;
                this._isTransactionOpen = false;
            }
        }

        public void Complete()
        {
            this.Consistent = true;
        }

        public IDbCommand CreateCommand(CommandType commandType)
        {
            IDbCommand command = this._dataSource.DbProvider.CreateCommand();
            command.CommandType = commandType;
            command.Connection = this._connection;
            if (this._transaction != null)
            {
                try
                {
                    command.Transaction = this._transaction;
                }
                catch
                {
                }
            }
            if (this._connection != null)
            {
                try
                {
                    command.CommandTimeout = this._connection.ConnectionTimeout;
                }
                catch (NotSupportedException exception)
                {
                    if (_logger.IsInfoEnabled)
                    {
                        _logger.Info(exception.Message);
                    }
                }
            }
            return command;
        }

        public void CreateConnection()
        {
            this.CreateConnection(this._dataSource.ConnectionString);
        }

        public void CreateConnection(string connectionString)
        {
            this._connection = this._dataSource.DbProvider.CreateConnection();
            this._connection.ConnectionString = connectionString;
        }

        public IDbDataAdapter CreateDataAdapter()
        {
            return this._dataSource.DbProvider.CreateDataAdapter();
        }

        public IDbDataAdapter CreateDataAdapter(IDbCommand command)
        {
            IDbDataAdapter adapter = null;
            adapter = this._dataSource.DbProvider.CreateDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }

        public IDbDataParameter CreateDataParameter()
        {
            return this._dataSource.DbProvider.CreateDataParameter();
        }

        public void Dispose()
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Dispose SqlMapSession");
            }
            if (!this._isTransactionOpen)
            {
                if (this._connection.State != ConnectionState.Closed)
                {
                    this._sqlMapper.CloseConnection();
                }
            }
            else if (this._consistent)
            {
                this._sqlMapper.CommitTransaction();
                this._isTransactionOpen = false;
            }
            else if (this._connection.State != ConnectionState.Closed)
            {
                this._sqlMapper.RollBackTransaction();
                this._isTransactionOpen = false;
            }
        }

        public void OpenConnection()
        {
            this.OpenConnection(this._dataSource.ConnectionString);
        }

        public void OpenConnection(string connectionString)
        {
            if (this._connection == null)
            {
                this.CreateConnection(connectionString);
                try
                {
                    this._connection.Open();
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.Debug(string.Format("Open Connection \"{0}\" to \"{1}\".", this._connection.GetHashCode().ToString(), this._dataSource.DbProvider.Description));
                    }
                    return;
                }
                catch (Exception exception)
                {
                    throw new DataMapperException(string.Format("Unable to open connection to \"{0}\".", this._dataSource.DbProvider.Description), exception);
                }
            }
            if (this._connection.State != ConnectionState.Open)
            {
                try
                {
                    this._connection.Open();
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.Debug(string.Format("Open Connection \"{0}\" to \"{1}\".", this._connection.GetHashCode().ToString(), this._dataSource.DbProvider.Description));
                    }
                }
                catch (Exception exception2)
                {
                    throw new DataMapperException(string.Format("Unable to open connection to \"{0}\".", this._dataSource.DbProvider.Description), exception2);
                }
            }
        }

        public void RollBackTransaction()
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("RollBack Transaction.");
            }
            this._transaction.Rollback();
            this._transaction.Dispose();
            this._transaction = null;
            this._isTransactionOpen = false;
            if (this._connection.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }

        public void RollBackTransaction(bool closeConnection)
        {
            if (closeConnection)
            {
                this.RollBackTransaction();
            }
            else
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("RollBack Transaction.");
                }
                this._transaction.Rollback();
                this._transaction.Dispose();
                this._transaction = null;
                this._isTransactionOpen = false;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return this._connection;
            }
        }

        private bool Consistent
        {
            set
            {
                this._consistent = value;
            }
        }

        public IDataSource DataSource
        {
            get
            {
                return this._dataSource;
            }
        }

        public bool IsTransactionStart
        {
            get
            {
                return this._isTransactionOpen;
            }
        }

        public ISqlMapper SqlMapper
        {
            get
            {
                return this._sqlMapper;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return this._transaction;
            }
        }
    }
}

