namespace IBatisNet.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ConfigurationException : IBatisNetException
    {
        public ConfigurationException() : base("Could not configure the iBatis.NET framework.")
        {
        }

        public ConfigurationException(Exception ex) : base(ex.Message, ex)
        {
        }

        public ConfigurationException(string message) : base(message)
        {
        }

        protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

