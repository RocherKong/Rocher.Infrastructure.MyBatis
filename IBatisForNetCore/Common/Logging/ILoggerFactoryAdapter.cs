namespace IBatisNet.Common.Logging
{
    using System;

    public interface ILoggerFactoryAdapter
    {
        ILog GetLogger(string name);
        ILog GetLogger(Type type);
    }
}

