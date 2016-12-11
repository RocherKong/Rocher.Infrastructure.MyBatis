namespace IBatisNet.DataMapper.Configuration.Statements
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("insert", Namespace="http://ibatis.apache.org/mapping")]
    public class Insert : Statement
    {
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.Statements.Generate _generate;
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.Statements.SelectKey _selectKey;

        [XmlIgnore]
        public override string ExtendStatement
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        [XmlElement("generate", typeof(IBatisNet.DataMapper.Configuration.Statements.Generate))]
        public IBatisNet.DataMapper.Configuration.Statements.Generate Generate
        {
            get
            {
                return this._generate;
            }
            set
            {
                this._generate = value;
            }
        }

        [XmlElement("selectKey", typeof(IBatisNet.DataMapper.Configuration.Statements.SelectKey))]
        public IBatisNet.DataMapper.Configuration.Statements.SelectKey SelectKey
        {
            get
            {
                return this._selectKey;
            }
            set
            {
                this._selectKey = value;
            }
        }
    }
}

