namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.MappedStatements.ArgumentStrategy;
    using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
    using IBatisNet.DataMapper.Proxy;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Reflection;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("result", Namespace="http://ibatis.apache.org/mapping")]
    public class ResultProperty
    {
        [NonSerialized]
        private static readonly IFactory _arrayListFactory = new ArrayListFactory();
        [NonSerialized]
        private string _callBackName = string.Empty;
        [NonSerialized]
        private string _clrType = string.Empty;
        [NonSerialized]
        private int _columnIndex = -999999;
        [NonSerialized]
        private string _columnName = string.Empty;
        [NonSerialized]
        private string _dbType;
        [NonSerialized]
        private bool _isComplexMemberName;
        [NonSerialized]
        private bool _isGenericIList;
        [NonSerialized]
        private bool _isIList;
        [NonSerialized]
        private bool _isLazyLoad;
        [NonSerialized]
        private ILazyFactory _lazyFactory;
        [NonSerialized]
        private IFactory _listFactory;
        [NonSerialized]
        private IResultMap _nestedResultMap;
        [NonSerialized]
        private string _nestedResultMapName = string.Empty;
        [NonSerialized]
        private string _nullValue;
        [NonSerialized]
        private string _propertyName = string.Empty;
        [NonSerialized]
        private IPropertyStrategy _propertyStrategy;
        [NonSerialized]
        private string _select = string.Empty;
        [NonSerialized]
        private ISetAccessor _setAccessor;
        [NonSerialized]
        private ITypeHandler _typeHandler;
        public const int UNKNOWN_COLUMN_INDEX = -999999;

        public ResultProperty Clone()
        {
            return new ResultProperty { CLRType = this.CLRType, CallBackName = this.CallBackName, ColumnIndex = this.ColumnIndex, ColumnName = this.ColumnName, DbType = this.DbType, IsLazyLoad = this.IsLazyLoad, NestedResultMapName = this.NestedResultMapName, NullValue = this.NullValue, PropertyName = this.PropertyName, Select = this.Select };
        }

        public object GetDataBaseValue(IDataReader dataReader)
        {
            object valueByName = null;
            if (this._columnIndex == -999999)
            {
                valueByName = this.TypeHandler.GetValueByName(this, dataReader);
            }
            else
            {
                valueByName = this.TypeHandler.GetValueByIndex(this, dataReader);
            }
            if (valueByName != DBNull.Value)
            {
                return valueByName;
            }
            if (this.HasNullValue)
            {
                if (this._setAccessor != null)
                {
                    return this.TypeHandler.ValueOf(this._setAccessor.MemberType, this._nullValue);
                }
                return this.TypeHandler.ValueOf(null, this._nullValue);
            }
            return this.TypeHandler.NullValue;
        }

        public void Initialize(ConfigurationScope configScope, Type resultClass)
        {
            if (((this._propertyName.Length > 0) && (this._propertyName != "value")) && !typeof(IDictionary).IsAssignableFrom(resultClass))
            {
                if (!this._isComplexMemberName)
                {
                    this._setAccessor = configScope.DataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(resultClass, this._propertyName);
                }
                else
                {
                    MemberInfo memberInfoForSetter = ObjectProbe.GetMemberInfoForSetter(resultClass, this._propertyName);
                    string name = this._propertyName.Substring(this._propertyName.LastIndexOf('.') + 1);
                    this._setAccessor = configScope.DataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(memberInfoForSetter.ReflectedType, name);
                }
                this._isGenericIList = TypeUtils.IsImplementGenericIListInterface(this.MemberType);
                this._isIList = typeof(IList).IsAssignableFrom(this.MemberType);
                if (this._isGenericIList)
                {
                    if (this.MemberType.IsArray)
                    {
                        this._listFactory = _arrayListFactory;
                    }
                    else
                    {
                        Type[] genericArguments = this.MemberType.GetGenericArguments();
                        if (genericArguments.Length == 0)
                        {
                            this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(this.MemberType, Type.EmptyTypes);
                        }
                        else
                        {
                            Type type2 = typeof(IList<>).MakeGenericType(genericArguments);
                            Type type3 = typeof(List<>);
                            Type type4 = type3.MakeGenericType(genericArguments);
                            if ((type2 == this.MemberType) || (type4 == this.MemberType))
                            {
                                Type typeToCreate = type3.MakeGenericType(genericArguments);
                                this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(typeToCreate, Type.EmptyTypes);
                            }
                            else
                            {
                                this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(this.MemberType, Type.EmptyTypes);
                            }
                        }
                    }
                }
                else if (this._isIList)
                {
                    if (this.MemberType.IsArray)
                    {
                        this._listFactory = _arrayListFactory;
                    }
                    else if (this.MemberType == typeof(IList))
                    {
                        this._listFactory = _arrayListFactory;
                    }
                    else
                    {
                        this._listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(this.MemberType, Type.EmptyTypes);
                    }
                }
            }
            if ((this.CallBackName != null) && (this.CallBackName.Length > 0))
            {
                configScope.ErrorContext.MoreInfo = "Result property '" + this._propertyName + "' check the typeHandler attribute '" + this.CallBackName + "' (must be a ITypeHandlerCallback implementation).";
                try
                {
                    ITypeHandlerCallback callback = (ITypeHandlerCallback) Activator.CreateInstance(configScope.SqlMapper.TypeHandlerFactory.GetType(this.CallBackName));
                    this._typeHandler = new CustomTypeHandler(callback);
                    goto Label_0320;
                }
                catch (Exception exception)
                {
                    throw new ConfigurationException("Error occurred during custom type handler configuration.  Cause: " + exception.Message, exception);
                }
            }
            configScope.ErrorContext.MoreInfo = "Result property '" + this._propertyName + "' set the typeHandler attribute.";
            this._typeHandler = configScope.ResolveTypeHandler(resultClass, this._propertyName, this._clrType, this._dbType, true);
        Label_0320:
            if (this.IsLazyLoad)
            {
                this._lazyFactory = new LazyFactoryBuilder().GetLazyFactory(this._setAccessor.MemberType);
            }
        }

        internal void Initialize(TypeHandlerFactory typeHandlerFactory, ISetAccessor setAccessor)
        {
            this._setAccessor = setAccessor;
            this._typeHandler = typeHandlerFactory.GetTypeHandler(setAccessor.MemberType);
        }

        public object TranslateValue(object value)
        {
            if (value == null)
            {
                return this.TypeHandler.NullValue;
            }
            return value;
        }

        [XmlIgnore]
        public virtual IArgumentStrategy ArgumentStrategy
        {
            get
            {
                throw new NotImplementedException("Valid on ArgumentProperty");
            }
            set
            {
                throw new NotImplementedException("Valid on ArgumentProperty");
            }
        }

        [XmlAttribute("typeHandler")]
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

        [XmlAttribute("type")]
        public string CLRType
        {
            get
            {
                return this._clrType;
            }
            set
            {
                this._clrType = value;
            }
        }

        [XmlAttribute("columnIndex")]
        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
            set
            {
                this._columnIndex = value;
            }
        }

        [XmlAttribute("column")]
        public string ColumnName
        {
            get
            {
                return this._columnName;
            }
            set
            {
                this._columnName = value;
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

        [XmlIgnore]
        public bool HasNullValue
        {
            get
            {
                return (this._nullValue != null);
            }
        }

        public bool IsComplexMemberName
        {
            get
            {
                return this._isComplexMemberName;
            }
        }

        [XmlIgnore]
        public bool IsGenericIList
        {
            get
            {
                return this._isGenericIList;
            }
        }

        [XmlIgnore]
        public bool IsIList
        {
            get
            {
                return this._isIList;
            }
        }

        [XmlAttribute("lazyLoad")]
        public virtual bool IsLazyLoad
        {
            get
            {
                return this._isLazyLoad;
            }
            set
            {
                this._isLazyLoad = value;
            }
        }

        [XmlIgnore]
        public ILazyFactory LazyFactory
        {
            get
            {
                return this._lazyFactory;
            }
        }

        [XmlIgnore]
        public IFactory ListFactory
        {
            get
            {
                return this._listFactory;
            }
        }

        [XmlIgnore]
        public virtual Type MemberType
        {
            get
            {
                if (this._setAccessor != null)
                {
                    return this._setAccessor.MemberType;
                }
                if (this._nestedResultMap == null)
                {
                    throw new IBatisNetException(string.Format(CultureInfo.InvariantCulture, "Could not resolve member type for result property '{0}'. Neither nested result map nor typed setter was provided.", new object[] { this._propertyName }));
                }
                return this._nestedResultMap.Class;
            }
        }

        [XmlIgnore]
        public IResultMap NestedResultMap
        {
            get
            {
                return this._nestedResultMap;
            }
            set
            {
                this._nestedResultMap = value;
            }
        }

        [XmlAttribute("resultMapping")]
        public string NestedResultMapName
        {
            get
            {
                return this._nestedResultMapName;
            }
            set
            {
                this._nestedResultMapName = value;
            }
        }

        [XmlAttribute("nullValue")]
        public string NullValue
        {
            get
            {
                return this._nullValue;
            }
            set
            {
                this._nullValue = value;
            }
        }

        [XmlAttribute("property")]
        public string PropertyName
        {
            get
            {
                return this._propertyName;
            }
            set
            {
                this._propertyName = value;
                if (this._propertyName.IndexOf('.') < 0)
                {
                    this._isComplexMemberName = false;
                }
                else
                {
                    this._isComplexMemberName = true;
                }
            }
        }

        [XmlIgnore]
        public IPropertyStrategy PropertyStrategy
        {
            get
            {
                return this._propertyStrategy;
            }
            set
            {
                this._propertyStrategy = value;
            }
        }

        [XmlAttribute("select")]
        public string Select
        {
            get
            {
                return this._select;
            }
            set
            {
                this._select = value;
            }
        }

        [XmlIgnore]
        public ISetAccessor SetAccessor
        {
            get
            {
                return this._setAccessor;
            }
        }

        [XmlIgnore]
        public ITypeHandler TypeHandler
        {
            get
            {
                if (this._typeHandler == null)
                {
                    throw new DataMapperException(string.Format("Error on Result property {0}, type handler for {1} is not registered.", this.PropertyName, this.MemberType.Name));
                }
                return this._typeHandler;
            }
            set
            {
                this._typeHandler = value;
            }
        }

        private class ArrayListFactory : IFactory
        {
            public object CreateInstance(object[] parameters)
            {
                return new ArrayList();
            }
        }
    }
}

