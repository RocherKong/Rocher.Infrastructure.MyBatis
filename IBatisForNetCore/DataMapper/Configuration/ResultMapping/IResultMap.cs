namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using IBatisNet.DataMapper.DataExchange;
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.Xml.Serialization;

    public interface IResultMap
    {
        object CreateInstanceOfResult(object[] parameters);
        IResultMap ResolveSubMap(IDataReader dataReader);
        void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue);

        [XmlIgnore]
        Type Class { get; }

        [XmlIgnore]
        IDataExchange DataExchange { set; }

        [XmlIgnore]
        ResultPropertyCollection GroupByProperties { get; }

        [XmlIgnore]
        StringCollection GroupByPropertyNames { get; }

        [XmlAttribute("id")]
        string Id { get; }

        [XmlIgnore]
        bool IsInitalized { get; set; }

        [XmlIgnore]
        ResultPropertyCollection Parameters { get; }

        [XmlIgnore]
        ResultPropertyCollection Properties { get; }
    }
}

