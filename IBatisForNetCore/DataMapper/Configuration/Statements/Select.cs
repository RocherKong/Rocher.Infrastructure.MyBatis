namespace IBatisNet.DataMapper.Configuration.Statements
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("select", Namespace="http://ibatis.apache.org/mapping")]
    public class Select : Statement
    {
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.Statements.Generate _generate;

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
    }
}

