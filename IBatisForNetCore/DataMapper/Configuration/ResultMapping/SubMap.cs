namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("subMap", Namespace="http://ibatis.apache.org/mapping")]
    public class SubMap
    {
        [NonSerialized]
        private string _discriminatorValue = string.Empty;
        [NonSerialized]
        private IResultMap _resultMap;
        [NonSerialized]
        private string _resultMapName = string.Empty;

        public SubMap(string discriminatorValue, string resultMapName)
        {
            this._discriminatorValue = discriminatorValue;
            this._resultMapName = resultMapName;
        }

        [XmlAttribute("value")]
        public string DiscriminatorValue
        {
            get
            {
                return this._discriminatorValue;
            }
        }

        [XmlIgnore]
        public IResultMap ResultMap
        {
            get
            {
                return this._resultMap;
            }
            set
            {
                this._resultMap = value;
            }
        }

        [XmlAttribute("resultMapping")]
        public string ResultMapName
        {
            get
            {
                return this._resultMapName;
            }
        }
    }
}

