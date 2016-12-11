namespace IBatisNet.DataMapper.Configuration.Statements
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("generate", Namespace="http://ibatis.apache.org/mapping")]
    public class Generate : Statement
    {
        [NonSerialized]
        private string _by = string.Empty;
        [NonSerialized]
        private string _table = string.Empty;

        [XmlAttribute("by")]
        public string By
        {
            get
            {
                return this._by;
            }
            set
            {
                this._by = value;
            }
        }

        [XmlAttribute("table")]
        public string Table
        {
            get
            {
                return this._table;
            }
            set
            {
                this._table = value;
            }
        }
    }
}

