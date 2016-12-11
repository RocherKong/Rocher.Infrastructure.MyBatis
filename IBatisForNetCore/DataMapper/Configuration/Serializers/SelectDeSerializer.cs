namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class SelectDeSerializer
    {
        public static Select Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Select select = new Select();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            select.CacheModelName = NodeUtils.GetStringAttribute(attributes, "cacheModel");
            select.ExtendStatement = NodeUtils.GetStringAttribute(attributes, "extends");
            select.Id = NodeUtils.GetStringAttribute(attributes, "id");
            select.ListClassName = NodeUtils.GetStringAttribute(attributes, "listClass");
            select.ParameterClassName = NodeUtils.GetStringAttribute(attributes, "parameterClass");
            select.ParameterMapName = NodeUtils.GetStringAttribute(attributes, "parameterMap");
            select.ResultClassName = NodeUtils.GetStringAttribute(attributes, "resultClass");
            select.ResultMapName = NodeUtils.GetStringAttribute(attributes, "resultMap");
            select.AllowRemapping = NodeUtils.GetBooleanAttribute(attributes, "remapResults", false);
            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "generate")
                {
                    Generate generate = new Generate();
                    NameValueCollection values2 = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
                    generate.By = NodeUtils.GetStringAttribute(values2, "by");
                    generate.Table = NodeUtils.GetStringAttribute(values2, "table");
                    select.Generate = generate;
                }
            }
            return select;
        }
    }
}

