﻿namespace IBatisNet.Common
{
    using System;
    using System.Data;

    public interface IDalSession : IDisposable
    {
        void BeginTransaction();
        void BeginTransaction(bool openConnection);
        void BeginTransaction(IsolationLevel isolationLevel);
        void BeginTransaction(string connectionString);
        void BeginTransaction(bool openConnection, IsolationLevel isolationLevel);
        void BeginTransaction(string connectionString, IsolationLevel isolationLevel);
        void BeginTransaction(string connectionString, bool openConnection, IsolationLevel isolationLevel);
        void CloseConnection();
        void CommitTransaction();
        void CommitTransaction(bool closeConnection);
        void Complete();
        IDbCommand CreateCommand(CommandType commandType);
        IDbDataAdapter CreateDataAdapter();
        IDbDataAdapter CreateDataAdapter(IDbCommand command);
        IDbDataParameter CreateDataParameter();
        void OpenConnection();
        void OpenConnection(string connectionString);
        void RollBackTransaction();
        void RollBackTransaction(bool closeConnection);

        IDbConnection Connection { get; }

        IDataSource DataSource { get; }

        bool IsTransactionStart { get; }

        IDbTransaction Transaction { get; }
    }
}

