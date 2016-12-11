namespace IBatisNet.DataMapper.Scope
{
    using IBatisNet.Common;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Xml;

    public class ConfigurationScope : IScope
    {
        private HybridDictionary _cacheModelFlushOnExecuteStatements = new HybridDictionary();
        private IBatisNet.Common.DataSource _dataSource;
        private IBatisNet.DataMapper.Scope.ErrorContext _errorContext = new IBatisNet.DataMapper.Scope.ErrorContext();
        private bool _isCacheModelsEnabled;
        private bool _isCallFromDao;
        private bool _isXmlValid = true;
        private XmlNode _nodeContext;
        private System.Xml.XmlNamespaceManager _nsmgr;
        private NameValueCollection _properties = new NameValueCollection();
        private HybridDictionary _providers = new HybridDictionary();
        private HybridDictionary _sqlIncludes = new HybridDictionary();
        private XmlDocument _sqlMapConfigDocument;
        private XmlDocument _sqlMapDocument;
        private string _sqlMapNamespace;
        private ISqlMapper _sqlMapper;
        private bool _useConfigFileWatcher;
        private bool _useReflectionOptimizer = true;
        private bool _useStatementNamespaces;
        private bool _validateSqlMap;
        public const string EMPTY_PARAMETER_MAP = "iBATIS.Empty.ParameterMap";

        public ConfigurationScope()
        {
            this._providers.Clear();
        }

        public string ApplyNamespace(string id)
        {
            string str = id;
            if ((((this._sqlMapNamespace != null) && (this._sqlMapNamespace.Length > 0)) && ((id != null) && (id.Length > 0))) && (id.IndexOf(".") < 0))
            {
                str = this._sqlMapNamespace + "." + id;
            }
            return str;
        }

        public ITypeHandler ResolveTypeHandler(Type clazz, string memberName, string clrType, string dbType, bool forSetter)
        {
            ITypeHandler typeHandler = null;
            if (clazz == null)
            {
                return this.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
            }
            if (typeof(IDictionary).IsAssignableFrom(clazz))
            {
                if ((clrType == null) || (clrType.Length == 0))
                {
                    return this.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
                try
                {
                    Type type = TypeUtils.ResolveType(clrType);
                    return this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
                }
                catch (Exception exception)
                {
                    throw new ConfigurationErrorsException("Error. Could not set TypeHandler.  Cause: " + exception.Message, exception);
                }
            }
            if (this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(clazz, dbType) != null)
            {
                return this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(clazz, dbType);
            }
            if ((clrType == null) || (clrType.Length == 0))
            {
                Type memberTypeForSetter = null;
                if (forSetter)
                {
                    memberTypeForSetter = ObjectProbe.GetMemberTypeForSetter(clazz, memberName);
                }
                else
                {
                    memberTypeForSetter = ObjectProbe.GetMemberTypeForGetter(clazz, memberName);
                }
                return this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(memberTypeForSetter, dbType);
            }
            try
            {
                Type type3 = TypeUtils.ResolveType(clrType);
                typeHandler = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type3, dbType);
            }
            catch (Exception exception2)
            {
                throw new ConfigurationErrorsException("Error. Could not set TypeHandler.  Cause: " + exception2.Message, exception2);
            }
            return typeHandler;
        }

        public HybridDictionary CacheModelFlushOnExecuteStatements
        {
            get
            {
                return this._cacheModelFlushOnExecuteStatements;
            }
            set
            {
                this._cacheModelFlushOnExecuteStatements = value;
            }
        }

        public IBatisNet.DataMapper.DataExchange.DataExchangeFactory DataExchangeFactory
        {
            get
            {
                return this._sqlMapper.DataExchangeFactory;
            }
        }

        public IBatisNet.Common.DataSource DataSource
        {
            get
            {
                return this._dataSource;
            }
            set
            {
                this._dataSource = value;
            }
        }

        public IBatisNet.DataMapper.Scope.ErrorContext ErrorContext
        {
            get
            {
                return this._errorContext;
            }
        }

        public bool IsCacheModelsEnabled
        {
            get
            {
                return this._isCacheModelsEnabled;
            }
            set
            {
                this._isCacheModelsEnabled = value;
            }
        }

        public bool IsCallFromDao
        {
            get
            {
                return this._isCallFromDao;
            }
            set
            {
                this._isCallFromDao = value;
            }
        }

        public bool IsXmlValid
        {
            get
            {
                return this._isXmlValid;
            }
            set
            {
                this._isXmlValid = value;
            }
        }

        public XmlNode NodeContext
        {
            get
            {
                return this._nodeContext;
            }
            set
            {
                this._nodeContext = value;
            }
        }

        public NameValueCollection Properties
        {
            get
            {
                return this._properties;
            }
        }

        public HybridDictionary Providers
        {
            get
            {
                return this._providers;
            }
        }

        public HybridDictionary SqlIncludes
        {
            get
            {
                return this._sqlIncludes;
            }
        }

        public XmlDocument SqlMapConfigDocument
        {
            get
            {
                return this._sqlMapConfigDocument;
            }
            set
            {
                this._sqlMapConfigDocument = value;
            }
        }

        public XmlDocument SqlMapDocument
        {
            get
            {
                return this._sqlMapDocument;
            }
            set
            {
                this._sqlMapDocument = value;
            }
        }

        public string SqlMapNamespace
        {
            get
            {
                return this._sqlMapNamespace;
            }
            set
            {
                this._sqlMapNamespace = value;
            }
        }

        public ISqlMapper SqlMapper
        {
            get
            {
                return this._sqlMapper;
            }
            set
            {
                this._sqlMapper = value;
            }
        }

        public bool UseConfigFileWatcher
        {
            get
            {
                return this._useConfigFileWatcher;
            }
            set
            {
                this._useConfigFileWatcher = value;
            }
        }

        public bool UseReflectionOptimizer
        {
            get
            {
                return this._useReflectionOptimizer;
            }
            set
            {
                this._useReflectionOptimizer = value;
            }
        }

        public bool UseStatementNamespaces
        {
            get
            {
                return this._useStatementNamespaces;
            }
            set
            {
                this._useStatementNamespaces = value;
            }
        }

        public bool ValidateSqlMap
        {
            get
            {
                return this._validateSqlMap;
            }
            set
            {
                this._validateSqlMap = value;
            }
        }

        public System.Xml.XmlNamespaceManager XmlNamespaceManager
        {
            get
            {
                return this._nsmgr;
            }
            set
            {
                this._nsmgr = value;
            }
        }
    }
}

