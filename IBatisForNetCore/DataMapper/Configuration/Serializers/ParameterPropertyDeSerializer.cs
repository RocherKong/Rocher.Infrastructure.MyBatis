namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class ParameterPropertyDeSerializer
    {
        public static ParameterProperty Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            ParameterProperty property = new ParameterProperty();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            configScope.ErrorContext.MoreInfo = "ParameterPropertyDeSerializer";
            property.CallBackName = NodeUtils.GetStringAttribute(attributes, "typeHandler");
            property.CLRType = NodeUtils.GetStringAttribute(attributes, "type");
            property.ColumnName = NodeUtils.GetStringAttribute(attributes, "column");
            property.DbType = NodeUtils.GetStringAttribute(attributes, "dbType", null);
            property.DirectionAttribute = NodeUtils.GetStringAttribute(attributes, "direction");
            property.NullValue = attributes["nullValue"];
            property.PropertyName = NodeUtils.GetStringAttribute(attributes, "property");
            property.Precision = NodeUtils.GetByteAttribute(attributes, "precision", 0);
            property.Scale = NodeUtils.GetByteAttribute(attributes, "scale", 0);
            property.Size = NodeUtils.GetIntAttribute(attributes, "size", -1);
            return property;
        }
    }
}

