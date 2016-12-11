namespace IBatisNet.Common
{
    using IBatisNet.Common.Xml;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class DataSourceDeSerializer
    {
        public static DataSource Deserialize(XmlNode node)
        {
            DataSource source = new DataSource();
            NameValueCollection values = NodeUtils.ParseAttributes(node);
            source.ConnectionString = values["connectionString"];
            source.Name = values["name"];
            return source;
        }
    }
}

