namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class ResultMapDeSerializer
    {
        public static ResultMap Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            NameValueCollection values = NodeUtils.ParseAttributes(node, configScope.Properties);
            ResultMap map = new ResultMap(configScope, values["id"], values["class"], values["extends"], values["groupBy"]);
            configScope.ErrorContext.MoreInfo = "initialize ResultMap";
            map.Initialize(configScope);
            return map;
        }
    }
}

