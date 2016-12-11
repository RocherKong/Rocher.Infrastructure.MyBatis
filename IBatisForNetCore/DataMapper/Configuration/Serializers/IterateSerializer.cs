namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class IterateSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope;

        public IterateSerializer(ConfigurationScope configScope)
        {
            this._configScope = configScope;
        }

        public SqlTag Deserialize(XmlNode node)
        {
            Iterate iterate = new Iterate(this._configScope.DataExchangeFactory.AccessorFactory);
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, this._configScope.Properties);
            iterate.Prepend = NodeUtils.GetStringAttribute(attributes, "prepend");
            iterate.Property = NodeUtils.GetStringAttribute(attributes, "property");
            iterate.Close = NodeUtils.GetStringAttribute(attributes, "close");
            iterate.Conjunction = NodeUtils.GetStringAttribute(attributes, "conjunction");
            iterate.Open = NodeUtils.GetStringAttribute(attributes, "open");
            return iterate;
        }
    }
}

