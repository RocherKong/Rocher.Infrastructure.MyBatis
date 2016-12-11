namespace IBatisNet.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class IBatisNetException : ApplicationException
    {
        public IBatisNetException() : base("iBatis.NET framework caused an exception.")
        {
        }

        public IBatisNetException(Exception ex) : base("iBatis.NET framework caused an exception.", ex)
        {
        }

        public IBatisNetException(string message) : base(message)
        {
        }

        protected IBatisNetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IBatisNetException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

