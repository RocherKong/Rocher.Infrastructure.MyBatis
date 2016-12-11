namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class IsNotEmptyDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope;

        public IsNotEmptyDeSerializer(ConfigurationScope configScope)
        {
            this._configScope = configScope;
        }

        public SqlTag Deserialize(XmlNode node)
        {
            IsNotEmpty empty = new IsNotEmpty(this._configScope.DataExchangeFactory.AccessorFactory);
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
            empty.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
            empty.Property = NodeUtils.GetStringAttribute(attributes, "property");
            return empty;
        }
    }
}

