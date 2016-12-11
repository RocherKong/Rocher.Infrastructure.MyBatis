namespace IBatisNet.Common.Transaction
{
    using IBatisNet.Common.Logging;
    using System;
    using System.EnterpriseServices;
    using System.Runtime.Remoting.Messaging;

    public class TransactionScope : IDisposable
    {
        private bool _closed;
        private bool _consistent;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private TransactionOptions _txOptions;
        private TransactionScopeOptions _txScopeOptions;
        private const string TX_SCOPE_COUNT = "_TX_SCOPE_COUNT_";

        public TransactionScope()
        {
            this._txOptions = new TransactionOptions();
            this._txOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            this._txOptions.TimeOut = new TimeSpan(0, 0, 0, 15);
            this._txScopeOptions = TransactionScopeOptions.Required;
            this.EnterTransactionContext();
        }

        public TransactionScope(TransactionScopeOptions txScopeOptions)
        {
            this._txOptions = new TransactionOptions();
            this._txOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            this._txOptions.TimeOut = new TimeSpan(0, 0, 0, 15);
            this._txScopeOptions = txScopeOptions;
            this.EnterTransactionContext();
        }

        public TransactionScope(TransactionScopeOptions txScopeOptions, TransactionOptions options)
        {
            this._txOptions = options;
            this._txScopeOptions = txScopeOptions;
            this.EnterTransactionContext();
        }

        public void Close()
        {
            if (!this._closed)
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("Close TransactionScope");
                }
                if (ContextUtil.IsInTransaction)
                {
                    if (this._consistent && this.IsVoteCommit)
                    {
                        ContextUtil.EnableCommit();
                    }
                    else
                    {
                        ContextUtil.DisableCommit();
                    }
                }
                if (--this.TransactionScopeCount == 0)
                {
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.Debug("Leave in ServiceDomain ");
                    }
                    ServiceDomain.Leave();
                }
                this._closed = true;
            }
        }

        public void Complete()
        {
            this.Consistent = true;
        }

        public void Dispose()
        {
            this.Close();
        }

        private void EnterTransactionContext()
        {
            if (++this.TransactionScopeCount == 1)
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("Create a new ServiceConfig in ServiceDomain.");
                }
                ServiceConfig cfg = new ServiceConfig {
                    TrackingEnabled = true,
                    TrackingAppName = "iBATIS.NET",
                    TrackingComponentName = "TransactionScope",
                    TransactionDescription = "iBATIS.NET Distributed Transaction",
                    Transaction = this.TransactionScopeOptions2TransactionOption(this._txScopeOptions),
                    TransactionTimeout = this._txOptions.TimeOut.Seconds,
                    IsolationLevel = this.IsolationLevel2TransactionIsolationLevel(this._txOptions.IsolationLevel)
                };
                ServiceDomain.Enter(cfg);
            }
            this._closed = false;
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Open TransactionScope :" + ContextUtil.ContextId);
            }
        }

        private TransactionIsolationLevel IsolationLevel2TransactionIsolationLevel(IsolationLevel isolation)
        {
            switch (isolation)
            {
                case IsolationLevel.Serializable:
                    return TransactionIsolationLevel.Serializable;

                case IsolationLevel.RepeatableRead:
                    return TransactionIsolationLevel.RepeatableRead;

                case IsolationLevel.ReadCommitted:
                    return TransactionIsolationLevel.ReadCommitted;

                case IsolationLevel.ReadUncommitted:
                    return TransactionIsolationLevel.ReadUncommitted;

                case IsolationLevel.Unspecified:
                    throw new NotImplementedException("Will be used in Indigo.");
            }
            return TransactionIsolationLevel.ReadCommitted;
        }

        private TransactionOption TransactionScopeOptions2TransactionOption(TransactionScopeOptions transactionScopeOptions)
        {
            switch (transactionScopeOptions)
            {
                case TransactionScopeOptions.Required:
                    return TransactionOption.Required;

                case TransactionScopeOptions.RequiresNew:
                    return TransactionOption.RequiresNew;

                case TransactionScopeOptions.Supported:
                    return TransactionOption.Supported;

                case TransactionScopeOptions.NotSupported:
                    return TransactionOption.NotSupported;

                case TransactionScopeOptions.Mandatory:
                    throw new NotImplementedException("Will be used in Indigo.");
            }
            return TransactionOption.Required;
        }

        private bool Consistent
        {
            set
            {
                this._consistent = value;
            }
        }

        public static bool IsInTransaction
        {
            get
            {
                return ContextUtil.IsInTransaction;
            }
        }

        public bool IsVoteCommit
        {
            get
            {
                return (ContextUtil.MyTransactionVote == TransactionVote.Commit);
            }
        }

        public int TransactionScopeCount
        {
            get
            {
                object data = CallContext.GetData("_TX_SCOPE_COUNT_");
                if (data != null)
                {
                    return (int) data;
                }
                return 0;
            }
            set
            {
                CallContext.SetData("_TX_SCOPE_COUNT_", value);
            }
        }
    }
}

