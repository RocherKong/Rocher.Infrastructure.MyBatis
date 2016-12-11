namespace IBatisNet.Common.Logging.Impl
{
    using IBatisNet.Common.Logging;
    using System;
    using System.Collections.Specialized;

    public sealed class NoOpLoggerFA : ILoggerFactoryAdapter
    {
        private ILog _nopLogger;

        public NoOpLoggerFA()
        {
            this._nopLogger = new NoOpLogger();
        }

        public NoOpLoggerFA(NameValueCollection properties)
        {
            this._nopLogger = new NoOpLogger();
        }

        public ILog GetLogger(Type type)
        {
            return this._nopLogger;
        }

        ILog ILoggerFactoryAdapter.GetLogger(string name)
        {
            return this._nopLogger;
        }
    }
}

