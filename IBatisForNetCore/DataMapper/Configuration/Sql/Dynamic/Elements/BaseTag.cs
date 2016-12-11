namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public abstract class BaseTag : SqlTag
    {
        [NonSerialized]
        private string _property = string.Empty;

        protected BaseTag()
        {
        }

        [XmlAttribute("property")]
        public string Property
        {
            get
            {
                return this._property;
            }
            set
            {
                this._property = value;
            }
        }
    }
}

