namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Xml;

    public sealed class SqlDeSerializer
    {
        public static void Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            string stringAttribute = NodeUtils.GetStringAttribute(NodeUtils.ParseAttributes(node, configScope.Properties), "id");
            if (configScope.UseStatementNamespaces)
            {
                stringAttribute = configScope.ApplyNamespace(stringAttribute);
            }
            if (configScope.SqlIncludes.Contains(stringAttribute))
            {
                throw new ConfigurationException("Duplicate <sql>-include '" + stringAttribute + "' found.");
            }
            configScope.SqlIncludes.Add(stringAttribute, node);
        }
    }
}

