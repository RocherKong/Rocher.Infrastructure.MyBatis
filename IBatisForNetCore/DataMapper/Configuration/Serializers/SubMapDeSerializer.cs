namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class SubMapDeSerializer
    {
        public static SubMap Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            string stringAttribute = NodeUtils.GetStringAttribute(attributes, "value");
            return new SubMap(stringAttribute, configScope.ApplyNamespace(NodeUtils.GetStringAttribute(attributes, "resultMapping")));
        }
    }
}

