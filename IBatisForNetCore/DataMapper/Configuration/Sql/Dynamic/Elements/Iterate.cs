namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("iterate", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class Iterate : BaseTag
    {
        [NonSerialized]
        private string _close = string.Empty;
        [NonSerialized]
        private string _conjunction = string.Empty;
        [NonSerialized]
        private string _open = string.Empty;

        public Iterate(AccessorFactory accessorFactory)
        {
            base.Handler = new IterateTagHandler(accessorFactory);
        }

        [XmlAttribute("close")]
        public string Close
        {
            get
            {
                return this._close;
            }
            set
            {
                this._close = value;
            }
        }

        [XmlAttribute("conjunction")]
        public string Conjunction
        {
            get
            {
                return this._conjunction;
            }
            set
            {
                this._conjunction = value;
            }
        }

        [XmlAttribute("open")]
        public string Open
        {
            get
            {
                return this._open;
            }
            set
            {
                this._open = value;
            }
        }
    }
}

