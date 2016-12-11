namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;
    using System.Collections;
    using System.Text;

    public sealed class SqlTagContext
    {
        private Hashtable _attributes = new Hashtable();
        private SqlTag _firstNonDynamicTagWithPrepend;
        private bool _overridePrepend = false;
        private ArrayList _parameterMappings = new ArrayList();
        private StringBuilder buffer = new StringBuilder();

        public void AddAttribute(object key, object value)
        {
            this._attributes.Add(key, value);
        }

        public void AddParameterMapping(ParameterProperty mapping)
        {
            this._parameterMappings.Add(mapping);
        }

        public object GetAttribute(object key)
        {
            return this._attributes[key];
        }

        public IList GetParameterMappings()
        {
            return this._parameterMappings;
        }

        public StringBuilder GetWriter()
        {
            return this.buffer;
        }

        public string BodyText
        {
            get
            {
                return this.buffer.ToString().Trim();
            }
        }

        public SqlTag FirstNonDynamicTagWithPrepend
        {
            get
            {
                return this._firstNonDynamicTagWithPrepend;
            }
            set
            {
                this._firstNonDynamicTagWithPrepend = value;
            }
        }

        public bool IsOverridePrepend
        {
            get
            {
                return this._overridePrepend;
            }
            set
            {
                this._overridePrepend = value;
            }
        }
    }
}

