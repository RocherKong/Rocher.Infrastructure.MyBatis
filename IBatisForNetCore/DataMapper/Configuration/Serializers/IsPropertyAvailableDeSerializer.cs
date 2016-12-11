namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class IsPropertyAvailableDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope;

        public IsPropertyAvailableDeSerializer(ConfigurationScope configScope)
        {
            this._configScope = configScope;
        }

        public SqlTag Deserialize(XmlNode node)
        {
            IsPropertyAvailable available = new IsPropertyAvailable(this._configScope.DataExchangeFactory.AccessorFactory);
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
            available.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
            available.Property = NodeUtils.GetStringAttribute(attributes, "property");
            return available;
        }
    }
}

