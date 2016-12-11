namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Cache;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class CacheModelDeSerializer
    {
        public static CacheModel Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            CacheModel model = new CacheModel();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            model.Id = NodeUtils.GetStringAttribute(attributes, "id");
            model.Implementation = NodeUtils.GetStringAttribute(attributes, "implementation");
            model.Implementation = configScope.SqlMapper.TypeHandlerFactory.GetTypeAlias(model.Implementation).Class.AssemblyQualifiedName;
            model.IsReadOnly = NodeUtils.GetBooleanAttribute(attributes, "readOnly", true);
            model.IsSerializable = NodeUtils.GetBooleanAttribute(attributes, "serialize", false);
            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "flushInterval")
                {
                    FlushInterval interval = new FlushInterval();
                    NameValueCollection values2 = NodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
                    interval.Hours = NodeUtils.GetIntAttribute(values2, "hours", 0);
                    interval.Milliseconds = NodeUtils.GetIntAttribute(values2, "milliseconds", 0);
                    interval.Minutes = NodeUtils.GetIntAttribute(values2, "minutes", 0);
                    interval.Seconds = NodeUtils.GetIntAttribute(values2, "seconds", 0);
                    model.FlushInterval = interval;
                }
            }
            return model;
        }
    }
}

