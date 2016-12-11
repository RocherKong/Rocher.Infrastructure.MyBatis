namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class ParameterMapDeSerializer
    {
        public static ParameterMap Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            ParameterMap map = new ParameterMap(configScope.DataExchangeFactory);
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            configScope.ErrorContext.MoreInfo = "ParameterMap DeSerializer";
            map.ExtendMap = NodeUtils.GetStringAttribute(attributes, "extends");
            map.Id = NodeUtils.GetStringAttribute(attributes, "id");
            map.ClassName = NodeUtils.GetStringAttribute(attributes, "class");
            configScope.ErrorContext.MoreInfo = "Initialize ParameterMap";
            configScope.NodeContext = node;
            map.Initialize(configScope.DataSource.DbProvider.UsePositionalParameters, configScope);
            map.BuildProperties(configScope);
            configScope.ErrorContext.MoreInfo = string.Empty;
            return map;
        }
    }
}

