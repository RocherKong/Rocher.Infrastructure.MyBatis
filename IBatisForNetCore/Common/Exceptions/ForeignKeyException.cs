namespace IBatisNet.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ForeignKeyException : IBatisNetException
    {
        public ForeignKeyException() : base("A foreign key conflict has occurred.")
        {
        }

        public ForeignKeyException(Exception ex) : base(ex.Message, ex)
        {
        }

        public ForeignKeyException(string message) : base(message)
        {
        }

        protected ForeignKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ForeignKeyException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

