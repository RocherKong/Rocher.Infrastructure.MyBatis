namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class UpdateDeSerializer
    {
        public static Update Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Update update = new Update();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            update.CacheModelName = NodeUtils.GetStringAttribute(attributes, "cacheModel");
            update.ExtendStatement = NodeUtils.GetStringAttribute(attributes, "extends");
            update.Id = NodeUtils.GetStringAttribute(attributes, "id");
            update.ParameterClassName = NodeUtils.GetStringAttribute(attributes, "parameterClass");
            update.ParameterMapName = NodeUtils.GetStringAttribute(attributes, "parameterMap");
            update.AllowRemapping = NodeUtils.GetBooleanAttribute(attributes, "remapResults", false);
            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "generate")
                {
                    Generate generate = new Generate();
                    NameValueCollection values2 = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
                    generate.By = NodeUtils.GetStringAttribute(values2, "by");
                    generate.Table = NodeUtils.GetStringAttribute(values2, "table");
                    update.Generate = generate;
                }
            }
            return update;
        }
    }
}

