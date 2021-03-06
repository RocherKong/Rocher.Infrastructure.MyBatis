﻿namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class IsParameterPresentDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope;

        public IsParameterPresentDeSerializer(ConfigurationScope configScope)
        {
            this._configScope = configScope;
        }

        public SqlTag Deserialize(XmlNode node)
        {
            IsParameterPresent present = new IsParameterPresent(this._configScope.DataExchangeFactory.AccessorFactory);
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
            present.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
            return present;
        }
    }
}

