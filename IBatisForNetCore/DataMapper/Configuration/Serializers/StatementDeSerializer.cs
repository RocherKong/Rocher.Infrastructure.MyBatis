namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class StatementDeSerializer
    {
        public static Statement Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Statement statement = new Statement();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            statement.CacheModelName = NodeUtils.GetStringAttribute(attributes, "cacheModel");
            statement.ExtendStatement = NodeUtils.GetStringAttribute(attributes, "extends");
            statement.Id = NodeUtils.GetStringAttribute(attributes, "id");
            statement.ListClassName = NodeUtils.GetStringAttribute(attributes, "listClass");
            statement.ParameterClassName = NodeUtils.GetStringAttribute(attributes, "parameterClass");
            statement.ParameterMapName = NodeUtils.GetStringAttribute(attributes, "parameterMap");
            statement.ResultClassName = NodeUtils.GetStringAttribute(attributes, "resultClass");
            statement.ResultMapName = NodeUtils.GetStringAttribute(attributes, "resultMap");
            statement.AllowRemapping = NodeUtils.GetBooleanAttribute(attributes, "remapResults", false);
            return statement;
        }
    }
}

