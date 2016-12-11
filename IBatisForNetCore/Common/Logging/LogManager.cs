namespace IBatisNet.Common.Logging
{
    using IBatisNet.Common.Logging.Impl;
    using System;
    using System.Configuration;

    public sealed class LogManager
    {
        private static ILoggerFactoryAdapter _adapter = null;
        private static object _loadLock = new object();
        private static readonly string IBATIS_SECTION_LOGGING = "iBATIS/logging";

        private LogManager()
        {
        }

        private static ILoggerFactoryAdapter BuildDefaultLoggerFactoryAdapter()
        {
            return new NoOpLoggerFA();
        }

        private static ILoggerFactoryAdapter BuildLoggerFactoryAdapter()
        {
            LogSetting section = null;
            try
            {
                section = (LogSetting) ConfigurationManager.GetSection(IBATIS_SECTION_LOGGING);
            }
            catch (Exception exception)
            {
                ILoggerFactoryAdapter adapter = BuildDefaultLoggerFactoryAdapter();
                adapter.GetLogger(typeof(LogManager)).Warn("Unable to read configuration. Using default logger.", exception);
                return adapter;
            }
            if ((section != null) && !typeof(ILoggerFactoryAdapter).IsAssignableFrom(section.FactoryAdapterType))
            {
                ILoggerFactoryAdapter adapter2 = BuildDefaultLoggerFactoryAdapter();
                adapter2.GetLogger(typeof(LogManager)).Warn("Type " + section.FactoryAdapterType.FullName + " does not implement ILoggerFactoryAdapter. Using default logger");
                return adapter2;
            }
            if (section != null)
            {
                if (section.Properties.Count > 0)
                {
                    try
                    {
                        object[] args = new object[] { section.Properties };
                        return (ILoggerFactoryAdapter) Activator.CreateInstance(section.FactoryAdapterType, args);
                    }
                    catch (Exception exception2)
                    {
                        ILoggerFactoryAdapter adapter4 = BuildDefaultLoggerFactoryAdapter();
                        adapter4.GetLogger(typeof(LogManager)).Warn("Unable to create instance of type " + section.FactoryAdapterType.FullName + ". Using default logger.", exception2);
                        return adapter4;
                    }
                }
                try
                {
                    return (ILoggerFactoryAdapter) Activator.CreateInstance(section.FactoryAdapterType);
                }
                catch (Exception exception3)
                {
                    ILoggerFactoryAdapter adapter5 = BuildDefaultLoggerFactoryAdapter();
                    adapter5.GetLogger(typeof(LogManager)).Warn("Unable to create instance of type " + section.FactoryAdapterType.FullName + ". Using default logger.", exception3);
                    return adapter5;
                }
            }
            ILoggerFactoryAdapter adapter6 = BuildDefaultLoggerFactoryAdapter();
            adapter6.GetLogger(typeof(LogManager)).Warn("Unable to read configuration IBatisNet/logging. Using default logger.");
            return adapter6;
        }

        public static ILog GetLogger(string name)
        {
            return Adapter.GetLogger(name);
        }

        public static ILog GetLogger(Type type)
        {
            return Adapter.GetLogger(type);
        }

        public static ILoggerFactoryAdapter Adapter
        {
            get
            {
                if (_adapter == null)
                {
                    lock (_loadLock)
                    {
                        if (_adapter == null)
                        {
                            _adapter = BuildLoggerFactoryAdapter();
                        }
                    }
                }
                return _adapter;
            }
            set
            {
                lock (_loadLock)
                {
                    _adapter = value;
                }
            }
        }
    }
}

