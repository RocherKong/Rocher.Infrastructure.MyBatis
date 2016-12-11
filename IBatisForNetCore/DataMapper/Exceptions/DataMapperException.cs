namespace IBatisNet.DataMapper.Exceptions
{
    using IBatisNet.Common.Exceptions;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class DataMapperException : IBatisNetException
    {
        public DataMapperException() : base("iBATIS.NET DataMapper component caused an exception.")
        {
        }

        public DataMapperException(Exception ex) : base("iBATIS.NET DataMapper component caused an exception.", ex)
        {
        }

        public DataMapperException(string message) : base(message)
        {
        }

        protected DataMapperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DataMapperException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

