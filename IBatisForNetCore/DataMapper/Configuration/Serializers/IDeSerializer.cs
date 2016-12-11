namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System.Xml;

    public interface IDeSerializer
    {
        SqlTag Deserialize(XmlNode node);
    }
}

