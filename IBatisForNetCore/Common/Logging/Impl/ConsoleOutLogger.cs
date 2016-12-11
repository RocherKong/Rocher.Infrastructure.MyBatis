namespace IBatisNet.Common.Logging.Impl
{
    using IBatisNet.Common.Logging;
    using System;
    using System.Globalization;
    using System.Text;

    public class ConsoleOutLogger : AbstractLogger
    {
        private IBatisNet.Common.Logging.LogLevel _currentLogLevel;
        private string _dateTimeFormat = string.Empty;
        private bool _hasDateTimeFormat;
        private string _logName = string.Empty;
        private bool _showDateTime;
        private bool _showLogName;

        public ConsoleOutLogger(string logName, IBatisNet.Common.Logging.LogLevel logLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            this._logName = logName;
            this._currentLogLevel = logLevel;
            this._showDateTime = showDateTime;
            this._showLogName = showLogName;
            this._dateTimeFormat = dateTimeFormat;
            if ((this._dateTimeFormat != null) && (this._dateTimeFormat.Length > 0))
            {
                this._hasDateTimeFormat = true;
            }
        }

        protected override bool IsLevelEnabled(IBatisNet.Common.Logging.LogLevel level)
        {
            int num = (int) level;
            int num2 = (int) this._currentLogLevel;
            return (num >= num2);
        }

        protected override void Write(IBatisNet.Common.Logging.LogLevel level, object message, Exception e)
        {
            StringBuilder builder = new StringBuilder();
            if (this._showDateTime)
            {
                if (this._hasDateTimeFormat)
                {
                    builder.Append(DateTime.Now.ToString(this._dateTimeFormat, CultureInfo.InvariantCulture));
                }
                else
                {
                    builder.Append(DateTime.Now);
                }
                builder.Append(" ");
            }
            builder.Append(string.Format("[{0}]", level.ToString().ToUpper()).PadRight(8));
            if (this._showLogName)
            {
                builder.Append(this._logName).Append(" - ");
            }
            builder.Append(message.ToString());
            if (e != null)
            {
                builder.Append(Environment.NewLine).Append(e.ToString());
            }
            Console.Out.WriteLine(builder.ToString());
        }
    }
}

