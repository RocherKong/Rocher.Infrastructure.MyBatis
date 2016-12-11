namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class ResultPropertyDeSerializer
    {
        public static ResultProperty Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            ResultProperty property = new ResultProperty();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            property.CLRType = NodeUtils.GetStringAttribute(attributes, "type");
            property.CallBackName = NodeUtils.GetStringAttribute(attributes, "typeHandler");
            property.ColumnIndex = NodeUtils.GetIntAttribute(attributes, "columnIndex", -999999);
            property.ColumnName = NodeUtils.GetStringAttribute(attributes, "column");
            property.DbType = NodeUtils.GetStringAttribute(attributes, "dbType");
            property.IsLazyLoad = NodeUtils.GetBooleanAttribute(attributes, "lazyLoad", false);
            property.NestedResultMapName = NodeUtils.GetStringAttribute(attributes, "resultMapping");
            property.NullValue = attributes["nullValue"];
            property.PropertyName = NodeUtils.GetStringAttribute(attributes, "property");
            property.Select = NodeUtils.GetStringAttribute(attributes, "select");
            return property;
        }
    }
}

