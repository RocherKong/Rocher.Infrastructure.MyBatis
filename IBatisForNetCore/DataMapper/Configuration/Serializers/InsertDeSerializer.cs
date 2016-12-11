namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class InsertDeSerializer
    {
        public static Insert Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Insert insert = new Insert();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            insert.CacheModelName = NodeUtils.GetStringAttribute(attributes, "cacheModel");
            insert.ExtendStatement = NodeUtils.GetStringAttribute(attributes, "extends");
            insert.Id = NodeUtils.GetStringAttribute(attributes, "id");
            insert.ParameterClassName = NodeUtils.GetStringAttribute(attributes, "parameterClass");
            insert.ParameterMapName = NodeUtils.GetStringAttribute(attributes, "parameterMap");
            insert.ResultClassName = NodeUtils.GetStringAttribute(attributes, "resultClass");
            insert.ResultMapName = NodeUtils.GetStringAttribute(attributes, "resultMap");
            insert.AllowRemapping = NodeUtils.GetBooleanAttribute(attributes, "remapResults", false);
            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "generate")
                {
                    Generate generate = new Generate();
                    NameValueCollection values2 = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
                    generate.By = NodeUtils.GetStringAttribute(values2, "by");
                    generate.Table = NodeUtils.GetStringAttribute(values2, "table");
                    insert.Generate = generate;
                }
                else if (node.ChildNodes[i].LocalName == "selectKey")
                {
                    SelectKey key = new SelectKey();
                    NameValueCollection values3 = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
                    key.PropertyName = NodeUtils.GetStringAttribute(values3, "property");
                    key.SelectKeyType = ReadSelectKeyType(values3["type"]);
                    key.ResultClassName = NodeUtils.GetStringAttribute(values3, "resultClass");
                    insert.SelectKey = key;
                }
            }
            return insert;
        }

        private static SelectKeyType ReadSelectKeyType(string s)
        {
            switch (s)
            {
                case "pre":
                    return SelectKeyType.pre;

                case "post":
                    return SelectKeyType.post;
            }
            throw new ConfigurationException("Unknown selectKey type : '" + s + "'");
        }
    }
}

