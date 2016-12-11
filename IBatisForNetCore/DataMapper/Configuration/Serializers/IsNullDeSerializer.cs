namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class IsNullDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope;

        public IsNullDeSerializer(ConfigurationScope configScope)
        {
            this._configScope = configScope;
        }

        public SqlTag Deserialize(XmlNode node)
        {
            IsNull @null = new IsNull(this._configScope.DataExchangeFactory.AccessorFactory);
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
            @null.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
            @null.Property = NodeUtils.GetStringAttribute(attributes, "property");
            return @null;
        }
    }
}

