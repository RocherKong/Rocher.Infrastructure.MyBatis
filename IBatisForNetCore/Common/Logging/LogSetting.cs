namespace IBatisNet.Common.Logging
{
    using System;
    using System.Collections.Specialized;

    internal class LogSetting
    {
        private Type _factoryAdapterType;
        private NameValueCollection _properties;

        public LogSetting(Type factoryAdapterType, NameValueCollection properties)
        {
            this._factoryAdapterType = factoryAdapterType;
            this._properties = properties;
        }

        public Type FactoryAdapterType
        {
            get
            {
                return this._factoryAdapterType;
            }
        }

        public NameValueCollection Properties
        {
            get
            {
                return this._properties;
            }
        }
    }
}

