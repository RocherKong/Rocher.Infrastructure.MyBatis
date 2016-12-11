namespace IBatisNet.Common.Logging.Impl
{
    using IBatisNet.Common.Logging;
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    public class ConsoleOutLoggerFA : ILoggerFactoryAdapter
    {
        private string _dateTimeFormat = string.Empty;
        private IBatisNet.Common.Logging.LogLevel _Level;
        private Hashtable _logs = Hashtable.Synchronized(new Hashtable());
        private bool _showDateTime = true;
        private bool _showLogName = true;

        public ConsoleOutLoggerFA(NameValueCollection properties)
        {
            try
            {
                this._Level = (IBatisNet.Common.Logging.LogLevel) Enum.Parse(typeof(IBatisNet.Common.Logging.LogLevel), properties["level"], true);
            }
            catch (Exception)
            {
                this._Level = IBatisNet.Common.Logging.LogLevel.All;
            }
            try
            {
                this._showDateTime = bool.Parse(properties["showDateTime"]);
            }
            catch (Exception)
            {
                this._showDateTime = true;
            }
            try
            {
                this._showLogName = bool.Parse(properties["showLogName"]);
            }
            catch (Exception)
            {
                this._showLogName = true;
            }
            this._dateTimeFormat = properties["dateTimeFormat"];
        }

        public ILog GetLogger(string name)
        {
            ILog log = this._logs[name] as ILog;
            if (log == null)
            {
                log = new ConsoleOutLogger(name, this._Level, this._showDateTime, this._showLogName, this._dateTimeFormat);
                this._logs.Add(name, log);
            }
            return log;
        }

        public ILog GetLogger(Type type)
        {
            return this.GetLogger(type.FullName);
        }
    }
}

