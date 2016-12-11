namespace IBatisNet.DataMapper.Commands
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public class DbCommandDecorator : IDbCommand, IDisposable
    {
        private IDbCommand _innerDbCommand;
        private RequestScope _request;

        public DbCommandDecorator(IDbCommand dbCommand, RequestScope request)
        {
            this._request = request;
            this._innerDbCommand = dbCommand;
        }

        void IDbCommand.Cancel()
        {
            this._innerDbCommand.Cancel();
        }

        IDbDataParameter IDbCommand.CreateParameter()
        {
            return this._innerDbCommand.CreateParameter();
        }

        int IDbCommand.ExecuteNonQuery()
        {
            this._request.Session.OpenConnection();
            return this._innerDbCommand.ExecuteNonQuery();
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            this._request.Session.OpenConnection();
            this._request.MoveNextResultMap();
            return new DataReaderDecorator(this._innerDbCommand.ExecuteReader(), this._request);
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            this._request.Session.OpenConnection();
            return this._innerDbCommand.ExecuteReader(behavior);
        }

        object IDbCommand.ExecuteScalar()
        {
            this._request.Session.OpenConnection();
            return this._innerDbCommand.ExecuteScalar();
        }

        void IDbCommand.Prepare()
        {
            this._innerDbCommand.Prepare();
        }

        void IDisposable.Dispose()
        {
            this._innerDbCommand.Dispose();
        }

        string IDbCommand.CommandText
        {
            get
            {
                return this._innerDbCommand.CommandText;
            }
            set
            {
                this._innerDbCommand.CommandText = value;
            }
        }

        int IDbCommand.CommandTimeout
        {
            get
            {
                return this._innerDbCommand.CommandTimeout;
            }
            set
            {
                this._innerDbCommand.CommandTimeout = value;
            }
        }

        CommandType IDbCommand.CommandType
        {
            get
            {
                return this._innerDbCommand.CommandType;
            }
            set
            {
                this._innerDbCommand.CommandType = value;
            }
        }

        IDbConnection IDbCommand.Connection
        {
            get
            {
                return this._innerDbCommand.Connection;
            }
            set
            {
                this._innerDbCommand.Connection = value;
            }
        }

        IDataParameterCollection IDbCommand.Parameters
        {
            get
            {
                return this._innerDbCommand.Parameters;
            }
        }

        IDbTransaction IDbCommand.Transaction
        {
            get
            {
                return this._innerDbCommand.Transaction;
            }
            set
            {
                this._innerDbCommand.Transaction = value;
            }
        }

        UpdateRowSource IDbCommand.UpdatedRowSource
        {
            get
            {
                return this._innerDbCommand.UpdatedRowSource;
            }
            set
            {
                this._innerDbCommand.UpdatedRowSource = value;
            }
        }
    }
}

