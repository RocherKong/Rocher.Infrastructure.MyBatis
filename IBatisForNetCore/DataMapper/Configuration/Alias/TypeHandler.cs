namespace IBatisNet.DataMapper.Configuration.Alias
{
    using IBatisNet.Common.Utilities;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("typeHandler", Namespace="http://ibatis.apache.org/dataMapper")]
    public class TypeHandler
    {
        [NonSerialized]
        private string _callBackName = string.Empty;
        [NonSerialized]
        private Type _class;
        [NonSerialized]
        private string _className = string.Empty;
        [NonSerialized]
        private string _dbType = string.Empty;

        public void Initialize()
        {
            this._class = TypeUtils.ResolveType(this._className);
        }

        [XmlAttribute("callback")]
        public string CallBackName
        {
            get
            {
                return this._callBackName;
            }
            set
            {
                this._callBackName = value;
            }
        }

        [XmlIgnore]
        public Type Class
        {
            get
            {
                return this._class;
            }
        }

        [XmlAttribute("type")]
        public string ClassName
        {
            get
            {
                return this._className;
            }
            set
            {
                this._className = value;
            }
        }

        [XmlAttribute("dbType")]
        public string DbType
        {
            get
            {
                return this._dbType;
            }
            set
            {
                this._dbType = value;
            }
        }
    }
}

