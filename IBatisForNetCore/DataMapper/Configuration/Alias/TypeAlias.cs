namespace IBatisNet.DataMapper.Configuration.Alias
{
    using IBatisNet.Common.Utilities;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("typeAlias", Namespace="http://ibatis.apache.org/dataMapper")]
    public class TypeAlias
    {
        [NonSerialized]
        private Type _class;
        [NonSerialized]
        private string _className;
        [NonSerialized]
        private string _name;

        public TypeAlias()
        {
            this._name = string.Empty;
            this._className = string.Empty;
        }

        public TypeAlias(Type type)
        {
            this._name = string.Empty;
            this._className = string.Empty;
            this._class = type;
        }

        public void Initialize()
        {
            this._class = TypeUtils.ResolveType(this._className);
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
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The class attribute is mandatory in the typeAlias " + this._name);
                }
                this._className = value;
            }
        }

        [XmlAttribute("alias")]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The name attribute is mandatory in the typeAlias ");
                }
                this._name = value;
            }
        }
    }
}

