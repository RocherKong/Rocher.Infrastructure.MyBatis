namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.DataExchange;
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.Xml.Serialization;

    public class AutoResultMap : IResultMap
    {
        [NonSerialized]
        private IDataExchange _dataExchange;
        [NonSerialized]
        private bool _isInitalized;
        [NonSerialized]
        private ResultPropertyCollection _properties = new ResultPropertyCollection();
        [NonSerialized]
        private Type _resultClass;
        [NonSerialized]
        private IFactory _resultClassFactory;

        public AutoResultMap(Type resultClass, IFactory resultClassFactory, IDataExchange dataExchange)
        {
            this._resultClass = resultClass;
            this._resultClassFactory = resultClassFactory;
            this._dataExchange = dataExchange;
        }

        public AutoResultMap Clone()
        {
            return new AutoResultMap(this._resultClass, this._resultClassFactory, this._dataExchange);
        }

        public object CreateInstanceOfResult(object[] parameters)
        {
            return this.CreateInstanceOfResultClass();
        }

        public object CreateInstanceOfResultClass()
        {
            if (this._resultClass.IsPrimitive || (this._resultClass == typeof(string)))
            {
                return TypeUtils.InstantiatePrimitiveType(Type.GetTypeCode(this._resultClass));
            }
            if (!this._resultClass.IsValueType)
            {
                return this._resultClassFactory.CreateInstance(null);
            }
            if (this._resultClass == typeof(DateTime))
            {
                return new DateTime();
            }
            if (this._resultClass == typeof(decimal))
            {
                return 0M;
            }
            if (this._resultClass == typeof(Guid))
            {
                return Guid.Empty;
            }
            if (this._resultClass == typeof(TimeSpan))
            {
                return new TimeSpan(0L);
            }
            if (!this._resultClass.IsGenericType || !typeof(Nullable<>).IsAssignableFrom(this._resultClass.GetGenericTypeDefinition()))
            {
                throw new NotImplementedException("Unable to instanciate value type");
            }
            return TypeUtils.InstantiateNullableType(this._resultClass);
        }

        public IResultMap ResolveSubMap(IDataReader dataReader)
        {
            return this;
        }

        public void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue)
        {
            this._dataExchange.SetData(ref target, property, dataBaseValue);
        }

        public Type Class
        {
            get
            {
                return this._resultClass;
            }
        }

        public IDataExchange DataExchange
        {
            set
            {
                this._dataExchange = value;
            }
        }

        public ResultPropertyCollection GroupByProperties
        {
            get
            {
                throw new NotImplementedException("The property 'GroupByProperties' is not implemented.");
            }
        }

        [XmlIgnore]
        public StringCollection GroupByPropertyNames
        {
            get
            {
                throw new NotImplementedException("The property 'GroupByPropertyNames' is not implemented.");
            }
        }

        public string Id
        {
            get
            {
                return this._resultClass.Name;
            }
        }

        public bool IsInitalized
        {
            get
            {
                return this._isInitalized;
            }
            set
            {
                this._isInitalized = value;
            }
        }

        [XmlIgnore]
        public ResultPropertyCollection Parameters
        {
            get
            {
                throw new NotImplementedException("The property 'Parameters' is not implemented.");
            }
        }

        [XmlIgnore]
        public ResultPropertyCollection Properties
        {
            get
            {
                return this._properties;
            }
        }
    }
}

