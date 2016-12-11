namespace IBatisNet.Common.Logging.Impl
{
    using IBatisNet.Common.Logging;
    using System;

    public sealed class NoOpLogger : ILog
    {
        public void Debug(object message)
        {
        }

        public void Debug(object message, Exception e)
        {
        }

        public void Error(object message)
        {
        }

        public void Error(object message, Exception e)
        {
        }

        public void Fatal(object message)
        {
        }

        public void Fatal(object message, Exception e)
        {
        }

        public void Info(object message)
        {
        }

        public void Info(object message, Exception e)
        {
        }

        public void Warn(object message)
        {
        }

        public void Warn(object message, Exception e)
        {
        }

        public bool IsDebugEnabled
        {
            get
            {
                return false;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return false;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return false;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return false;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return false;
            }
        }
    }
}

