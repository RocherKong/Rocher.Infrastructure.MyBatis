namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class DeleteDeSerializer
    {
        public static Delete Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Delete delete = new Delete();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            delete.CacheModelName = NodeUtils.GetStringAttribute(attributes, "cacheModel");
            delete.ExtendStatement = NodeUtils.GetStringAttribute(attributes, "extends");
            delete.Id = NodeUtils.GetStringAttribute(attributes, "id");
            delete.ListClassName = NodeUtils.GetStringAttribute(attributes, "listClass");
            delete.ParameterClassName = NodeUtils.GetStringAttribute(attributes, "parameterClass");
            delete.ParameterMapName = NodeUtils.GetStringAttribute(attributes, "parameterMap");
            delete.ResultClassName = NodeUtils.GetStringAttribute(attributes, "resultClass");
            delete.ResultMapName = NodeUtils.GetStringAttribute(attributes, "resultMap");
            delete.AllowRemapping = NodeUtils.GetBooleanAttribute(attributes, "remapResults", false);
            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "generate")
                {
                    Generate generate = new Generate();
                    NameValueCollection values2 = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
                    generate.By = NodeUtils.GetStringAttribute(values2, "by");
                    generate.Table = NodeUtils.GetStringAttribute(values2, "table");
                    delete.Generate = generate;
                }
            }
            return delete;
        }
    }
}

