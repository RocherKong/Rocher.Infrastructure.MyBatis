namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Collections;
    using System.Xml.Serialization;

    [Serializable]
    public abstract class SqlTag : ISqlChild, IDynamicParent
    {
        [NonSerialized]
        private IList _children = new ArrayList();
        [NonSerialized]
        private ISqlTagHandler _handler;
        [NonSerialized]
        private SqlTag _parent;
        [NonSerialized]
        private string _prepend = string.Empty;

        protected SqlTag()
        {
        }

        public void AddChild(ISqlChild child)
        {
            if (child is SqlTag)
            {
                ((SqlTag) child).Parent = this;
            }
            this._children.Add(child);
        }

        public IEnumerator GetChildrenEnumerator()
        {
            return this._children.GetEnumerator();
        }

        [XmlIgnore]
        public ISqlTagHandler Handler
        {
            get
            {
                return this._handler;
            }
            set
            {
                this._handler = value;
            }
        }

        public bool IsPrependAvailable
        {
            get
            {
                return ((this._prepend != null) && (this._prepend.Length > 0));
            }
        }

        [XmlIgnore]
        public SqlTag Parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                this._parent = value;
            }
        }

        [XmlAttribute("prepend")]
        public string Prepend
        {
            get
            {
                return this._prepend;
            }
            set
            {
                this._prepend = value;
            }
        }
    }
}

