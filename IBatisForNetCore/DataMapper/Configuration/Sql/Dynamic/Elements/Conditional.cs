namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public abstract class Conditional : BaseTag
    {
        [NonSerialized]
        private string _compareProperty = string.Empty;
        [NonSerialized]
        private string _compareValue = string.Empty;

        protected Conditional()
        {
        }

        [XmlAttribute("compareProperty")]
        public string CompareProperty
        {
            get
            {
                return this._compareProperty;
            }
            set
            {
                this._compareProperty = value;
            }
        }

        [XmlAttribute("compareValue")]
        public string CompareValue
        {
            get
            {
                return this._compareValue;
            }
            set
            {
                this._compareValue = value;
            }
        }
    }
}

