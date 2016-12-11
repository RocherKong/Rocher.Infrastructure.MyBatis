namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using IBatisNet.DataMapper.DataExchange;
    using System;
    using System.Collections.Specialized;
    using System.Data;

    public class NullResultMap : IResultMap
    {
        [NonSerialized]
        private ResultPropertyCollection _groupByProperties = new ResultPropertyCollection();
        [NonSerialized]
        private StringCollection _groupByPropertyNames = new StringCollection();
        [NonSerialized]
        private ResultPropertyCollection _parameters = new ResultPropertyCollection();
        [NonSerialized]
        private ResultPropertyCollection _properties = new ResultPropertyCollection();

        public object CreateInstanceOfResult(object[] parameters)
        {
            return null;
        }

        public IResultMap ResolveSubMap(IDataReader dataReader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Type Class
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IDataExchange DataExchange
        {
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IBatisNet.DataMapper.Configuration.ResultMapping.Discriminator Discriminator
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public string ExtendMap
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public ResultPropertyCollection GroupByProperties
        {
            get
            {
                return this._groupByProperties;
            }
        }

        public StringCollection GroupByPropertyNames
        {
            get
            {
                return this._groupByPropertyNames;
            }
        }

        public string Id
        {
            get
            {
                return "NullResultMap.Id";
            }
        }

        public bool IsInitalized
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public ResultPropertyCollection Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        public ResultPropertyCollection Properties
        {
            get
            {
                return this._properties;
            }
        }
    }
}

