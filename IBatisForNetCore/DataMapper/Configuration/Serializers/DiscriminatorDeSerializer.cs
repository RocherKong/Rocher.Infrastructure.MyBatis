namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class DiscriminatorDeSerializer
    {
        public static Discriminator Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Discriminator discriminator = new Discriminator();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            discriminator.CallBackName = NodeUtils.GetStringAttribute(attributes, "typeHandler");
            discriminator.CLRType = NodeUtils.GetStringAttribute(attributes, "type");
            discriminator.ColumnIndex = NodeUtils.GetIntAttribute(attributes, "columnIndex", -999999);
            discriminator.ColumnName = NodeUtils.GetStringAttribute(attributes, "column");
            discriminator.DbType = NodeUtils.GetStringAttribute(attributes, "dbType");
            discriminator.NullValue = attributes["nullValue"];
            return discriminator;
        }
    }
}

