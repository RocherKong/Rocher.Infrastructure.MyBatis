namespace IBatisNet.DataMapper.Configuration.Statements
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.Configuration.Cache;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Configuration.Sql;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("statement", Namespace="http://ibatis.apache.org/mapping")]
    public class Statement : IStatement
    {
        [NonSerialized]
        private bool _allowRemapping;
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.Cache.CacheModel _cacheModel;
        [NonSerialized]
        private string _cacheModelName = string.Empty;
        [NonSerialized]
        private string _extendStatement = string.Empty;
        [NonSerialized]
        private string _id = string.Empty;
        [NonSerialized]
        private Type _listClass;
        [NonSerialized]
        private IFactory _listClassFactory;
        [NonSerialized]
        private string _listClassName = string.Empty;
        [NonSerialized]
        private Type _parameterClass;
        [NonSerialized]
        private string _parameterClassName = string.Empty;
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.ParameterMapping.ParameterMap _parameterMap;
        [NonSerialized]
        private string _parameterMapName = string.Empty;
        [NonSerialized]
        private Type _resultClass;
        [NonSerialized]
        private string _resultClassName = string.Empty;
        [NonSerialized]
        private string _resultMapName = string.Empty;
        [NonSerialized]
        private ResultMapCollection _resultsMap = new ResultMapCollection();
        [NonSerialized]
        private ISql _sql;

        public IList<T> CreateInstanceOfGenericListClass<T>()
        {
            return (IList<T>) this._listClassFactory.CreateInstance(null);
        }

        public IList CreateInstanceOfListClass()
        {
            return (IList) this._listClassFactory.CreateInstance(null);
        }

        internal virtual void Initialize(ConfigurationScope configurationScope)
        {
            if (this._resultMapName.Length > 0)
            {
                string[] strArray = this._resultMapName.Split(new char[] { ',' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    string name = configurationScope.ApplyNamespace(strArray[i].Trim());
                    this._resultsMap.Add(configurationScope.SqlMapper.GetResultMap(name));
                }
            }
            if (this._parameterMapName.Length > 0)
            {
                this._parameterMap = configurationScope.SqlMapper.GetParameterMap(this._parameterMapName);
            }
            if (this._resultClassName.Length > 0)
            {
                string[] strArray2 = this._resultClassName.Split(new char[] { ',' });
                for (int j = 0; j < strArray2.Length; j++)
                {
                    this._resultClass = configurationScope.SqlMapper.TypeHandlerFactory.GetType(strArray2[j].Trim());
                    IFactory resultClassFactory = null;
                    if ((Type.GetTypeCode(this._resultClass) == TypeCode.Object) && !this._resultClass.IsValueType)
                    {
                        resultClassFactory = configurationScope.SqlMapper.ObjectFactory.CreateFactory(this._resultClass, Type.EmptyTypes);
                    }
                    IDataExchange dataExchangeForClass = configurationScope.DataExchangeFactory.GetDataExchangeForClass(this._resultClass);
                    IResultMap map = new AutoResultMap(this._resultClass, resultClassFactory, dataExchangeForClass);
                    this._resultsMap.Add(map);
                }
            }
            if (this._parameterClassName.Length > 0)
            {
                this._parameterClass = configurationScope.SqlMapper.TypeHandlerFactory.GetType(this._parameterClassName);
            }
            if (this._listClassName.Length > 0)
            {
                this._listClass = configurationScope.SqlMapper.TypeHandlerFactory.GetType(this._listClassName);
                this._listClassFactory = configurationScope.SqlMapper.ObjectFactory.CreateFactory(this._listClass, Type.EmptyTypes);
            }
        }

        [XmlAttribute("remapResults")]
        public bool AllowRemapping
        {
            get
            {
                return this._allowRemapping;
            }
            set
            {
                this._allowRemapping = value;
            }
        }

        [XmlIgnore]
        public IBatisNet.DataMapper.Configuration.Cache.CacheModel CacheModel
        {
            get
            {
                return this._cacheModel;
            }
            set
            {
                this._cacheModel = value;
            }
        }

        [XmlAttribute("cacheModel")]
        public string CacheModelName
        {
            get
            {
                return this._cacheModelName;
            }
            set
            {
                this._cacheModelName = value;
            }
        }

        [XmlIgnore]
        public virtual System.Data.CommandType CommandType
        {
            get
            {
                return System.Data.CommandType.Text;
            }
        }

        [XmlAttribute("extends")]
        public virtual string ExtendStatement
        {
            get
            {
                return this._extendStatement;
            }
            set
            {
                this._extendStatement = value;
            }
        }

        [XmlIgnore]
        public bool HasCacheModel
        {
            get
            {
                return (this._cacheModelName.Length > 0);
            }
        }

        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return this._id;
            }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new DataMapperException("The id attribute is required in a statement tag.");
                }
                this._id = value;
            }
        }

        [XmlIgnore]
        public Type ListClass
        {
            get
            {
                return this._listClass;
            }
        }

        [XmlAttribute("listClass")]
        public string ListClassName
        {
            get
            {
                return this._listClassName;
            }
            set
            {
                this._listClassName = value;
            }
        }

        [XmlIgnore]
        public Type ParameterClass
        {
            get
            {
                return this._parameterClass;
            }
        }

        [XmlAttribute("parameterClass")]
        public string ParameterClassName
        {
            get
            {
                return this._parameterClassName;
            }
            set
            {
                this._parameterClassName = value;
            }
        }

        [XmlIgnore]
        public IBatisNet.DataMapper.Configuration.ParameterMapping.ParameterMap ParameterMap
        {
            get
            {
                return this._parameterMap;
            }
            set
            {
                this._parameterMap = value;
            }
        }

        [XmlAttribute("parameterMap")]
        public string ParameterMapName
        {
            get
            {
                return this._parameterMapName;
            }
            set
            {
                this._parameterMapName = value;
            }
        }

        [XmlIgnore]
        public Type ResultClass
        {
            get
            {
                return this._resultClass;
            }
        }

        [XmlAttribute("resultClass")]
        public string ResultClassName
        {
            get
            {
                return this._resultClassName;
            }
            set
            {
                this._resultClassName = value;
            }
        }

        [XmlAttribute("resultMap")]
        public string ResultMapName
        {
            get
            {
                return this._resultMapName;
            }
            set
            {
                this._resultMapName = value;
            }
        }

        [XmlIgnore]
        public ResultMapCollection ResultsMap
        {
            get
            {
                return this._resultsMap;
            }
        }

        [XmlIgnore]
        public ISql Sql
        {
            get
            {
                return this._sql;
            }
            set
            {
                if (value == null)
                {
                    throw new DataMapperException("The sql statement query text is required in the statement tag " + this._id);
                }
                this._sql = value;
            }
        }
    }
}

