namespace IBatisNet.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ProbeException : IBatisNetException
    {
        public ProbeException() : base("A foreign key conflict has occurred.")
        {
        }

        public ProbeException(Exception ex) : base(ex.Message, ex)
        {
        }

        public ProbeException(string message) : base(message)
        {
        }

        protected ProbeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ProbeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

