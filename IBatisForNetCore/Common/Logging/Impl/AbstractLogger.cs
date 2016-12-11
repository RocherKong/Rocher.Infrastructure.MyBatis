namespace IBatisNet.Common.Logging.Impl
{
    using IBatisNet.Common.Logging;
    using System;

    public abstract class AbstractLogger : ILog
    {
        protected AbstractLogger()
        {
        }

        public void Debug(object message)
        {
            this.Debug(message, null);
        }

        public void Debug(object message, Exception e)
        {
            if (this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Debug))
            {
                this.Write(IBatisNet.Common.Logging.LogLevel.Debug, message, e);
            }
        }

        public void Error(object message)
        {
            this.Error(message, null);
        }

        public void Error(object message, Exception e)
        {
            if (this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Error))
            {
                this.Write(IBatisNet.Common.Logging.LogLevel.Error, message, e);
            }
        }

        public void Fatal(object message)
        {
            this.Fatal(message, null);
        }

        public void Fatal(object message, Exception e)
        {
            if (this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Fatal))
            {
                this.Write(IBatisNet.Common.Logging.LogLevel.Fatal, message, e);
            }
        }

        public void Info(object message)
        {
            this.Info(message, null);
        }

        public void Info(object message, Exception e)
        {
            if (this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Info))
            {
                this.Write(IBatisNet.Common.Logging.LogLevel.Info, message, e);
            }
        }

        protected abstract bool IsLevelEnabled(IBatisNet.Common.Logging.LogLevel logLevel);
        public void Warn(object message)
        {
            this.Warn(message, null);
        }

        public void Warn(object message, Exception e)
        {
            if (this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Warn))
            {
                this.Write(IBatisNet.Common.Logging.LogLevel.Warn, message, e);
            }
        }

        protected abstract void Write(IBatisNet.Common.Logging.LogLevel logLevel, object message, Exception e);

        public bool IsDebugEnabled
        {
            get
            {
                return this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Debug);
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Error);
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Fatal);
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Info);
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return this.IsLevelEnabled(IBatisNet.Common.Logging.LogLevel.Warn);
            }
        }
    }
}

