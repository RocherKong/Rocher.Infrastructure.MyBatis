namespace IBatisNet.Common
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("dataSource", Namespace="http://ibatis.apache.org/dataMapper")]
    public class DataSource : IDataSource
    {
        [NonSerialized]
        private string _connectionString = string.Empty;
        [NonSerialized]
        private string _name = string.Empty;
        [NonSerialized]
        private IDbProvider _provider;

        private void CheckPropertyString(string propertyName, string value)
        {
            if ((value == null) || (value.Trim().Length == 0))
            {
                throw new ArgumentException("The " + propertyName + " property cannot be set to a null or empty string value.", propertyName);
            }
        }

        public override string ToString()
        {
            return ("Source: ConnectionString : " + this.ConnectionString);
        }

        [XmlAttribute("connectionString")]
        public virtual string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                this.CheckPropertyString("ConnectionString", value);
                this._connectionString = value;
            }
        }

        [XmlIgnore]
        public virtual IDbProvider DbProvider
        {
            get
            {
                return this._provider;
            }
            set
            {
                this._provider = value;
            }
        }

        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this.CheckPropertyString("Name", value);
                this._name = value;
            }
        }
    }
}

