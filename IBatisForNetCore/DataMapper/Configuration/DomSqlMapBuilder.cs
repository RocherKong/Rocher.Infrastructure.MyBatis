namespace IBatisNet.DataMapper.Configuration
{
    using IBatisNet.Common;
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Alias;
    using IBatisNet.DataMapper.Configuration.Cache;
    using IBatisNet.DataMapper.Configuration.Cache.Fifo;
    using IBatisNet.DataMapper.Configuration.Cache.Lru;
    using IBatisNet.DataMapper.Configuration.Cache.Memory;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Configuration.Serializers;
    using IBatisNet.DataMapper.Configuration.Sql;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Configuration.Sql.SimpleDynamic;
    using IBatisNet.DataMapper.Configuration.Sql.Static;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.MappedStatements.ArgumentStrategy;
    using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Schema;

    public class DomSqlMapBuilder
    {
        private ConfigurationScope _configScope = new ConfigurationScope();
        private DeSerializerFactory _deSerializerFactory;
        private IGetAccessorFactory _getAccessorFactory;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IObjectFactory _objectFactory;
        private InlineParameterMapParser _paramParser = new InlineParameterMapParser();
        private ISetAccessorFactory _setAccessorFactory;
        private ISqlMapper _sqlMapper;
        private bool _validateSqlMapConfig = true;
        private const string ATR_CACHE_MODELS_ENABLED = "cacheModelsEnabled";
        private const string ATR_USE_REFLECTION_OPTIMIZER = "useReflectionOptimizer";
        private const string ATR_USE_STATEMENT_NAMESPACES = "useStatementNamespaces";
        private const string ATR_VALIDATE_SQLMAP = "validateSqlMap";
        private const string DATAMAPPER_NAMESPACE_PREFIX = "mapper";
        private const string DATAMAPPER_XML_NAMESPACE = "http://ibatis.apache.org/dataMapper";
        public const string DEFAULT_FILE_CONFIG_NAME = "SqlMap.config";
        private const string DEFAULT_PROVIDER_NAME = "_DEFAULT_PROVIDER_NAME";
        public const string DOT = ".";
        private const string MAPPING_NAMESPACE_PREFIX = "mapping";
        private const string MAPPING_XML_NAMESPACE = "http://ibatis.apache.org/mapping";
        private const string PROPERTY_ELEMENT_KEY_ATTRIB = "key";
        private const string PROPERTY_ELEMENT_VALUE_ATTRIB = "value";
        private const string PROVIDER_XML_NAMESPACE = "http://ibatis.apache.org/providers";
        private const string PROVIDERS_FILE_NAME = "providers.config";
        private const string PROVIDERS_NAMESPACE_PREFIX = "provider";
        private const string SQL_STATEMENT = "sqlMap/statements/sql";
        private const string XML_CACHE_MODEL = "sqlMap/cacheModels/cacheModel";
        private const string XML_CONFIG_PROVIDERS = "sqlMapConfig/providers";
        private const string XML_CONFIG_SETTINGS = "sqlMapConfig/settings/setting";
        private const string XML_DATABASE_DATASOURCE = "sqlMapConfig/database/dataSource";
        private const string XML_DATABASE_PROVIDER = "sqlMapConfig/database/provider";
        private const string XML_DATAMAPPER_CONFIG_ROOT = "sqlMapConfig";
        private const string XML_DELETE = "sqlMap/statements/delete";
        private const string XML_FLUSH_ON_EXECUTE = "flushOnExecute";
        private const string XML_GLOBAL_PROPERTIES = "*/add";
        private const string XML_GLOBAL_TYPEALIAS = "sqlMapConfig/alias/typeAlias";
        private const string XML_GLOBAL_TYPEHANDLER = "sqlMapConfig/typeHandlers/typeHandler";
        private const string XML_INSERT = "sqlMap/statements/insert";
        private const string XML_MAPPING_ROOT = "sqlMap";
        private const string XML_PARAMETERMAP = "sqlMap/parameterMaps/parameterMap";
        private const string XML_PROCEDURE = "sqlMap/statements/procedure";
        private const string XML_PROPERTIES = "properties";
        private const string XML_PROPERTY = "property";
        private const string XML_PROVIDER = "providers/provider";
        private const string XML_RESULTMAP = "sqlMap/resultMaps/resultMap";
        private const string XML_SEARCH_PARAMETER = "sqlMap/parameterMaps/parameterMap[@id='";
        private const string XML_SEARCH_RESULTMAP = "sqlMap/resultMaps/resultMap[@id='";
        private const string XML_SEARCH_STATEMENT = "sqlMap/statements";
        private const string XML_SELECT = "sqlMap/statements/select";
        private const string XML_SELECTKEY = "selectKey";
        private const string XML_SETTINGS_ADD = "/*/add";
        private const string XML_SQLMAP = "sqlMapConfig/sqlMaps/sqlMap";
        private const string XML_STATEMENT = "sqlMap/statements/statement";
        private const string XML_TYPEALIAS = "sqlMap/alias/typeAlias";
        private const string XML_UPDATE = "sqlMap/statements/update";

        public DomSqlMapBuilder()
        {
            this._deSerializerFactory = new DeSerializerFactory(this._configScope);
        }

        public string ApplyDataMapperNamespacePrefix(string elementName)
        {
            return ("mapper:" + elementName.Replace("/", "/mapper:"));
        }

        private void ApplyInlineParemeterMap(IStatement statement, string sqlStatement)
        {
            string str = sqlStatement;
            this._configScope.ErrorContext.MoreInfo = "apply inline parameterMap";
            if (statement.ParameterMap == null)
            {
                SqlText text = this._paramParser.ParseInlineParameterMap(this._configScope, statement, str);
                if (text.Parameters.Length > 0)
                {
                    ParameterMap map = new ParameterMap(this._configScope.DataExchangeFactory) {
                        Id = statement.Id + "-InLineParameterMap"
                    };
                    if (statement.ParameterClass != null)
                    {
                        map.Class = statement.ParameterClass;
                    }
                    map.Initialize(this._configScope.DataSource.DbProvider.UsePositionalParameters, this._configScope);
                    if (((statement.ParameterClass == null) && (text.Parameters.Length == 1)) && (text.Parameters[0].PropertyName == "value"))
                    {
                        map.DataExchange = this._configScope.DataExchangeFactory.GetDataExchangeForClass(typeof(int));
                    }
                    statement.ParameterMap = map;
                    int length = text.Parameters.Length;
                    for (int i = 0; i < length; i++)
                    {
                        map.AddParameterProperty(text.Parameters[i]);
                    }
                }
                str = text.Text;
            }
            ISql sql = null;
            str = str.Trim();
            if (SimpleDynamicSql.IsSimpleDynamicSql(str))
            {
                sql = new SimpleDynamicSql(this._configScope, str, statement);
            }
            else if (statement is Procedure)
            {
                sql = new ProcedureSql(this._configScope, str, statement);
            }
            else if (statement is Statement)
            {
                sql = new StaticSql(this._configScope, statement);
                ISqlMapSession session = new SqlMapSession(this._configScope.SqlMapper);
                ((StaticSql) sql).BuildPreparedStatement(session, str);
            }
            statement.Sql = sql;
        }

        public static string ApplyMappingNamespacePrefix(string elementName)
        {
            return ("mapping:" + elementName.Replace("/", "/mapping:"));
        }

        public string ApplyProviderNamespacePrefix(string elementName)
        {
            return ("provider:" + elementName.Replace("/", "/provider:"));
        }

        public ISqlMapper Build(XmlDocument document, bool useConfigFileWatcher)
        {
            return this.Build(document, null, useConfigFileWatcher, false);
        }

        private ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, bool isCallFromDao)
        {
            ISqlMapper sqlMapper;
            this._configScope.SqlMapConfigDocument = document;
            this._configScope.DataSource = dataSource;
            this._configScope.IsCallFromDao = isCallFromDao;
            this._configScope.UseConfigFileWatcher = useConfigFileWatcher;
            this._configScope.XmlNamespaceManager = new XmlNamespaceManager(this._configScope.SqlMapConfigDocument.NameTable);
            this._configScope.XmlNamespaceManager.AddNamespace("mapper", "http://ibatis.apache.org/dataMapper");
            this._configScope.XmlNamespaceManager.AddNamespace("provider", "http://ibatis.apache.org/providers");
            this._configScope.XmlNamespaceManager.AddNamespace("mapping", "http://ibatis.apache.org/mapping");
            try
            {
                if (this._validateSqlMapConfig)
                {
                    this.ValidateSchema(document.ChildNodes[1], "SqlMapConfig.xsd");
                }
                this.Initialize();
                sqlMapper = this._configScope.SqlMapper;
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(this._configScope.ErrorContext.ToString(), exception);
            }
            return sqlMapper;
        }

        public ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, NameValueCollection properties)
        {
            this._configScope.Properties.Add(properties);
            return this.Build(document, dataSource, useConfigFileWatcher, true);
        }

        private void BuildParameterMap()
        {
            XmlNode nodeContext = this._configScope.NodeContext;
            this._configScope.ErrorContext.MoreInfo = "build ParameterMap";
            string key = this._configScope.ApplyNamespace(nodeContext.Attributes.GetNamedItem("id").Value);
            this._configScope.ErrorContext.ObjectId = key;
            if (!this._configScope.SqlMapper.ParameterMaps.Contains(key))
            {
                ParameterMap parameterMap = ParameterMapDeSerializer.Deserialize(nodeContext, this._configScope);
                parameterMap.Id = this._configScope.ApplyNamespace(parameterMap.Id);
                string extendMap = parameterMap.ExtendMap;
                parameterMap.ExtendMap = this._configScope.ApplyNamespace(parameterMap.ExtendMap);
                if (parameterMap.ExtendMap.Length > 0)
                {
                    ParameterMap map2;
                    if (!this._configScope.SqlMapper.ParameterMaps.Contains(parameterMap.ExtendMap))
                    {
                        XmlNode node2 = this._configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix("sqlMap/parameterMaps/parameterMap[@id='") + extendMap + "']", this._configScope.XmlNamespaceManager);
                        if (node2 == null)
                        {
                            throw new ConfigurationException("In mapping file '" + this._configScope.SqlMapNamespace + "' the parameterMap '" + parameterMap.Id + "' can not resolve extends attribute '" + parameterMap.ExtendMap + "'");
                        }
                        this._configScope.ErrorContext.MoreInfo = "Build parent ParameterMap";
                        this._configScope.NodeContext = node2;
                        this.BuildParameterMap();
                        map2 = this._configScope.SqlMapper.GetParameterMap(parameterMap.ExtendMap);
                    }
                    else
                    {
                        map2 = this._configScope.SqlMapper.GetParameterMap(parameterMap.ExtendMap);
                    }
                    int index = 0;
                    foreach (string str3 in map2.GetPropertyNameArray())
                    {
                        ParameterProperty property = map2.GetProperty(str3).Clone();
                        property.Initialize(this._configScope, parameterMap.Class);
                        parameterMap.InsertParameterProperty(index, property);
                        index++;
                    }
                }
                this._configScope.SqlMapper.AddParameterMap(parameterMap);
            }
        }

        private void BuildResultMap()
        {
            XmlNode nodeContext = this._configScope.NodeContext;
            this._configScope.ErrorContext.MoreInfo = "build ResultMap";
            string key = this._configScope.ApplyNamespace(nodeContext.Attributes.GetNamedItem("id").Value);
            this._configScope.ErrorContext.ObjectId = key;
            if (!this._configScope.SqlMapper.ResultMaps.Contains(key))
            {
                ResultMap resultMap = ResultMapDeSerializer.Deserialize(nodeContext, this._configScope);
                string extendMap = resultMap.ExtendMap;
                resultMap.ExtendMap = this._configScope.ApplyNamespace(resultMap.ExtendMap);
                if ((resultMap.ExtendMap != null) && (resultMap.ExtendMap.Length > 0))
                {
                    IResultMap map2 = null;
                    if (!this._configScope.SqlMapper.ResultMaps.Contains(resultMap.ExtendMap))
                    {
                        XmlNode node2 = this._configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix("sqlMap/resultMaps/resultMap[@id='") + extendMap + "']", this._configScope.XmlNamespaceManager);
                        if (node2 == null)
                        {
                            throw new ConfigurationException("In mapping file '" + this._configScope.SqlMapNamespace + "' the resultMap '" + resultMap.Id + "' can not resolve extends attribute '" + resultMap.ExtendMap + "'");
                        }
                        this._configScope.ErrorContext.MoreInfo = "Build parent ResultMap";
                        this._configScope.NodeContext = node2;
                        this.BuildResultMap();
                        map2 = this._configScope.SqlMapper.GetResultMap(resultMap.ExtendMap);
                    }
                    else
                    {
                        map2 = this._configScope.SqlMapper.GetResultMap(resultMap.ExtendMap);
                    }
                    for (int i = 0; i < map2.Properties.Count; i++)
                    {
                        ResultProperty property = map2.Properties[i].Clone();
                        property.Initialize(this._configScope, resultMap.Class);
                        resultMap.Properties.Add(property);
                    }
                    if (resultMap.GroupByPropertyNames.Count == 0)
                    {
                        for (int k = 0; k < map2.GroupByPropertyNames.Count; k++)
                        {
                            resultMap.GroupByPropertyNames.Add(map2.GroupByPropertyNames[k]);
                        }
                    }
                    if (resultMap.Parameters.Count == 0)
                    {
                        for (int m = 0; m < map2.Parameters.Count; m++)
                        {
                            resultMap.Parameters.Add(map2.Parameters[m]);
                        }
                        if (resultMap.Parameters.Count > 0)
                        {
                            resultMap.SetObjectFactory(this._configScope);
                        }
                    }
                    for (int j = 0; j < resultMap.GroupByPropertyNames.Count; j++)
                    {
                        string propertyName = resultMap.GroupByPropertyNames[j];
                        if (!resultMap.Properties.Contains(propertyName))
                        {
                            throw new ConfigurationException(string.Format("Could not configure ResultMap named \"{0}\". Check the groupBy attribute. Cause: there's no result property named \"{1}\".", resultMap.Id, propertyName));
                        }
                    }
                }
                resultMap.InitializeGroupByProperties();
                this._configScope.SqlMapper.AddResultMap(resultMap);
            }
        }

        public ISqlMapper Configure()
        {
            return this.Configure(Resources.GetConfigAsXmlDocument("SqlMap.config"));
        }

        public ISqlMapper Configure(FileInfo resource)
        {
            XmlDocument fileInfoAsXmlDocument = Resources.GetFileInfoAsXmlDocument(resource);
            return this.Build(fileInfoAsXmlDocument, false);
        }

        public ISqlMapper Configure(Stream resource)
        {
            XmlDocument streamAsXmlDocument = Resources.GetStreamAsXmlDocument(resource);
            return this.Build(streamAsXmlDocument, false);
        }

        public ISqlMapper Configure(string resource)
        {
            XmlDocument urlAsXmlDocument;
            if (resource.StartsWith("file://"))
            {
                urlAsXmlDocument = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7));
            }
            else
            {
                urlAsXmlDocument = Resources.GetResourceAsXmlDocument(resource);
            }
            return this.Build(urlAsXmlDocument, false);
        }

        public ISqlMapper Configure(Uri resource)
        {
            XmlDocument uriAsXmlDocument = Resources.GetUriAsXmlDocument(resource);
            return this.Build(uriAsXmlDocument, false);
        }

        public ISqlMapper Configure(XmlDocument document)
        {
            return this.Build(document, false);
        }

        public ISqlMapper ConfigureAndWatch(ConfigureHandler configureDelegate)
        {
            return this.ConfigureAndWatch("SqlMap.config", configureDelegate);
        }

        public ISqlMapper ConfigureAndWatch(FileInfo resource, ConfigureHandler configureDelegate)
        {
            XmlDocument fileInfoAsXmlDocument = Resources.GetFileInfoAsXmlDocument(resource);
            ConfigWatcherHandler.ClearFilesMonitored();
            ConfigWatcherHandler.AddFileToWatch(resource);
            TimerCallback onWhatchedFileChange = new TimerCallback(DomSqlMapBuilder.OnConfigFileChange);
            StateConfig state = new StateConfig {
                FileName = resource.FullName,
                ConfigureHandler = configureDelegate
            };
            ISqlMapper mapper = this.Build(fileInfoAsXmlDocument, true);
            new ConfigWatcherHandler(onWhatchedFileChange, state);
            return mapper;
        }

        public ISqlMapper ConfigureAndWatch(string resource, ConfigureHandler configureDelegate)
        {
            XmlDocument urlAsXmlDocument = null;
            if (resource.StartsWith("file://"))
            {
                urlAsXmlDocument = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7));
            }
            else
            {
                urlAsXmlDocument = Resources.GetResourceAsXmlDocument(resource);
            }
            ConfigWatcherHandler.ClearFilesMonitored();
            ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(resource));
            TimerCallback onWhatchedFileChange = new TimerCallback(DomSqlMapBuilder.OnConfigFileChange);
            StateConfig state = new StateConfig {
                FileName = resource,
                ConfigureHandler = configureDelegate
            };
            ISqlMapper mapper = this.Build(urlAsXmlDocument, true);
            new ConfigWatcherHandler(onWhatchedFileChange, state);
            return mapper;
        }

        private void ConfigureSqlMap()
        {
            XmlNode nodeContext = this._configScope.NodeContext;
            this._configScope.ErrorContext.Activity = "loading SqlMap";
            this._configScope.ErrorContext.Resource = nodeContext.OuterXml.ToString();
            if (this._configScope.UseConfigFileWatcher && ((nodeContext.Attributes["resource"] != null) || (nodeContext.Attributes["url"] != null)))
            {
                ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(Resources.GetValueOfNodeResourceUrl(nodeContext, this._configScope.Properties)));
            }
            this._configScope.SqlMapDocument = Resources.GetAsXmlDocument(nodeContext, this._configScope.Properties);
            if (this._configScope.ValidateSqlMap)
            {
                this.ValidateSchema(this._configScope.SqlMapDocument.ChildNodes[1], "SqlMap.xsd");
            }
            this._configScope.SqlMapNamespace = this._configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix("sqlMap"), this._configScope.XmlNamespaceManager).Attributes["namespace"].Value;
            foreach (XmlNode node2 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/alias/typeAlias"), this._configScope.XmlNamespaceManager))
            {
                TypeAliasDeSerializer.Deserialize(node2, this._configScope);
            }
            this._configScope.ErrorContext.MoreInfo = string.Empty;
            this._configScope.ErrorContext.ObjectId = string.Empty;
            foreach (XmlNode node3 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/resultMaps/resultMap"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading ResultMap tag";
                this._configScope.NodeContext = node3;
                this.BuildResultMap();
            }
            foreach (XmlNode node4 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/parameterMaps/parameterMap"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading ParameterMap tag";
                this._configScope.NodeContext = node4;
                this.BuildParameterMap();
            }
            foreach (XmlNode node5 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/statements/sql"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading sql tag";
                this._configScope.NodeContext = node5;
                SqlDeSerializer.Deserialize(node5, this._configScope);
            }
            foreach (XmlNode node6 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/statements/statement"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading statement tag";
                this._configScope.NodeContext = node6;
                Statement statement = StatementDeSerializer.Deserialize(node6, this._configScope);
                statement.CacheModelName = this._configScope.ApplyNamespace(statement.CacheModelName);
                statement.ParameterMapName = this._configScope.ApplyNamespace(statement.ParameterMapName);
                if (this._configScope.UseStatementNamespaces)
                {
                    statement.Id = this._configScope.ApplyNamespace(statement.Id);
                }
                this._configScope.ErrorContext.ObjectId = statement.Id;
                statement.Initialize(this._configScope);
                this.ProcessSqlStatement(statement);
                MappedStatement statement2 = new MappedStatement(this._configScope.SqlMapper, statement);
                IMappedStatement mappedStatement = statement2;
                if (((statement.CacheModelName != null) && (statement.CacheModelName.Length > 0)) && this._configScope.IsCacheModelsEnabled)
                {
                    mappedStatement = new CachingStatement(statement2);
                }
                this._configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
            }
            foreach (XmlNode node7 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/statements/select"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading select tag";
                this._configScope.NodeContext = node7;
                Select select = SelectDeSerializer.Deserialize(node7, this._configScope);
                select.CacheModelName = this._configScope.ApplyNamespace(select.CacheModelName);
                select.ParameterMapName = this._configScope.ApplyNamespace(select.ParameterMapName);
                if (this._configScope.UseStatementNamespaces)
                {
                    select.Id = this._configScope.ApplyNamespace(select.Id);
                }
                this._configScope.ErrorContext.ObjectId = select.Id;
                select.Initialize(this._configScope);
                if (select.Generate != null)
                {
                    this.GenerateCommandText(this._configScope, select);
                }
                else
                {
                    this.ProcessSqlStatement(select);
                }
                MappedStatement statement4 = new SelectMappedStatement(this._configScope.SqlMapper, select);
                IMappedStatement statement5 = statement4;
                if (((select.CacheModelName != null) && (select.CacheModelName.Length > 0)) && this._configScope.IsCacheModelsEnabled)
                {
                    statement5 = new CachingStatement(statement4);
                }
                this._configScope.SqlMapper.AddMappedStatement(statement5.Id, statement5);
            }
            foreach (XmlNode node8 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/statements/insert"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading insert tag";
                this._configScope.NodeContext = node8;
                Insert insert = InsertDeSerializer.Deserialize(node8, this._configScope);
                insert.CacheModelName = this._configScope.ApplyNamespace(insert.CacheModelName);
                insert.ParameterMapName = this._configScope.ApplyNamespace(insert.ParameterMapName);
                if (this._configScope.UseStatementNamespaces)
                {
                    insert.Id = this._configScope.ApplyNamespace(insert.Id);
                }
                this._configScope.ErrorContext.ObjectId = insert.Id;
                insert.Initialize(this._configScope);
                if (insert.Generate != null)
                {
                    this.GenerateCommandText(this._configScope, insert);
                }
                else
                {
                    this.ProcessSqlStatement(insert);
                }
                MappedStatement statement6 = new InsertMappedStatement(this._configScope.SqlMapper, insert);
                this._configScope.SqlMapper.AddMappedStatement(statement6.Id, statement6);
                if (insert.SelectKey != null)
                {
                    this._configScope.ErrorContext.MoreInfo = "loading selectKey tag";
                    this._configScope.NodeContext = node8.SelectSingleNode(ApplyMappingNamespacePrefix("selectKey"), this._configScope.XmlNamespaceManager);
                    insert.SelectKey.Id = insert.Id;
                    insert.SelectKey.Initialize(this._configScope);
                    SelectKey selectKey = insert.SelectKey;
                    selectKey.Id = selectKey.Id + ".SelectKey";
                    this.ProcessSqlStatement(insert.SelectKey);
                    statement6 = new MappedStatement(this._configScope.SqlMapper, insert.SelectKey);
                    this._configScope.SqlMapper.AddMappedStatement(statement6.Id, statement6);
                }
            }
            foreach (XmlNode node9 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/statements/update"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading update tag";
                this._configScope.NodeContext = node9;
                Update update = UpdateDeSerializer.Deserialize(node9, this._configScope);
                update.CacheModelName = this._configScope.ApplyNamespace(update.CacheModelName);
                update.ParameterMapName = this._configScope.ApplyNamespace(update.ParameterMapName);
                if (this._configScope.UseStatementNamespaces)
                {
                    update.Id = this._configScope.ApplyNamespace(update.Id);
                }
                this._configScope.ErrorContext.ObjectId = update.Id;
                update.Initialize(this._configScope);
                if (update.Generate != null)
                {
                    this.GenerateCommandText(this._configScope, update);
                }
                else
                {
                    this.ProcessSqlStatement(update);
                }
                MappedStatement statement7 = new UpdateMappedStatement(this._configScope.SqlMapper, update);
                this._configScope.SqlMapper.AddMappedStatement(statement7.Id, statement7);
            }
            foreach (XmlNode node10 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/statements/delete"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading delete tag";
                this._configScope.NodeContext = node10;
                Delete delete = DeleteDeSerializer.Deserialize(node10, this._configScope);
                delete.CacheModelName = this._configScope.ApplyNamespace(delete.CacheModelName);
                delete.ParameterMapName = this._configScope.ApplyNamespace(delete.ParameterMapName);
                if (this._configScope.UseStatementNamespaces)
                {
                    delete.Id = this._configScope.ApplyNamespace(delete.Id);
                }
                this._configScope.ErrorContext.ObjectId = delete.Id;
                delete.Initialize(this._configScope);
                if (delete.Generate != null)
                {
                    this.GenerateCommandText(this._configScope, delete);
                }
                else
                {
                    this.ProcessSqlStatement(delete);
                }
                MappedStatement statement8 = new DeleteMappedStatement(this._configScope.SqlMapper, delete);
                this._configScope.SqlMapper.AddMappedStatement(statement8.Id, statement8);
            }
            foreach (XmlNode node11 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/statements/procedure"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.MoreInfo = "loading procedure tag";
                this._configScope.NodeContext = node11;
                Procedure procedure = ProcedureDeSerializer.Deserialize(node11, this._configScope);
                procedure.CacheModelName = this._configScope.ApplyNamespace(procedure.CacheModelName);
                procedure.ParameterMapName = this._configScope.ApplyNamespace(procedure.ParameterMapName);
                if (this._configScope.UseStatementNamespaces)
                {
                    procedure.Id = this._configScope.ApplyNamespace(procedure.Id);
                }
                this._configScope.ErrorContext.ObjectId = procedure.Id;
                procedure.Initialize(this._configScope);
                this.ProcessSqlStatement(procedure);
                MappedStatement statement9 = new MappedStatement(this._configScope.SqlMapper, procedure);
                IMappedStatement statement10 = statement9;
                if (((procedure.CacheModelName != null) && (procedure.CacheModelName.Length > 0)) && this._configScope.IsCacheModelsEnabled)
                {
                    statement10 = new CachingStatement(statement9);
                }
                this._configScope.SqlMapper.AddMappedStatement(statement10.Id, statement10);
            }
            if (this._configScope.IsCacheModelsEnabled)
            {
                foreach (XmlNode node12 in this._configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix("sqlMap/cacheModels/cacheModel"), this._configScope.XmlNamespaceManager))
                {
                    CacheModel cache = CacheModelDeSerializer.Deserialize(node12, this._configScope);
                    cache.Id = this._configScope.ApplyNamespace(cache.Id);
                    foreach (XmlNode node13 in node12.SelectNodes(ApplyMappingNamespacePrefix("flushOnExecute"), this._configScope.XmlNamespaceManager))
                    {
                        string id = node13.Attributes["statement"].Value;
                        if (this._configScope.UseStatementNamespaces)
                        {
                            id = this._configScope.ApplyNamespace(id);
                        }
                        IList list = (IList) this._configScope.CacheModelFlushOnExecuteStatements[cache.Id];
                        if (list == null)
                        {
                            list = new ArrayList();
                        }
                        list.Add(id);
                        this._configScope.CacheModelFlushOnExecuteStatements[cache.Id] = list;
                    }
                    foreach (XmlNode node14 in node12.SelectNodes(ApplyMappingNamespacePrefix("property"), this._configScope.XmlNamespaceManager))
                    {
                        string name = node14.Attributes["name"].Value;
                        string str3 = node14.Attributes["value"].Value;
                        cache.AddProperty(name, str3);
                    }
                    cache.Initialize();
                    this._configScope.SqlMapper.AddCache(cache);
                }
            }
            this._configScope.ErrorContext.Reset();
        }

        private void GenerateCommandText(ConfigurationScope configScope, IStatement statement)
        {
            string sqlStatement = SqlGenerator.BuildQuery(statement);
            ISql sql = new StaticSql(configScope, statement);
            ISqlMapSession session = new SqlMapSession(configScope.SqlMapper);
            ((StaticSql) sql).BuildPreparedStatement(session, sqlStatement);
            statement.Sql = sql;
        }

        private void GetProviders()
        {
            XmlDocument asXmlDocument;
            this._configScope.ErrorContext.Activity = "loading Providers";
            XmlNode node = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/providers"), this._configScope.XmlNamespaceManager);
            if (node != null)
            {
                asXmlDocument = Resources.GetAsXmlDocument(node, this._configScope.Properties);
            }
            else
            {
                asXmlDocument = Resources.GetConfigAsXmlDocument("providers.config");
            }
            foreach (XmlNode node2 in asXmlDocument.SelectNodes(this.ApplyProviderNamespacePrefix("providers/provider"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.Resource = node2.InnerXml.ToString();
                IDbProvider provider = ProviderDeSerializer.Deserialize(node2);
                if (provider.IsEnabled)
                {
                    this._configScope.ErrorContext.ObjectId = provider.Name;
                    this._configScope.ErrorContext.MoreInfo = "initialize provider";
                    provider.Initialize();
                    this._configScope.Providers.Add(provider.Name, provider);
                    if (provider.IsDefault)
                    {
                        if (this._configScope.Providers["_DEFAULT_PROVIDER_NAME"] != null)
                        {
                            throw new ConfigurationException(string.Format("Error while configuring the Provider named \"{0}\" There can be only one default Provider.", provider.Name));
                        }
                        this._configScope.Providers.Add("_DEFAULT_PROVIDER_NAME", provider);
                    }
                }
            }
            this._configScope.ErrorContext.Reset();
        }

        public Stream GetStream(string schemaResourceKey)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("IBatisNet.DataMapper." + schemaResourceKey);
        }

        private void Initialize()
        {
            this.Reset();
            if (!this._configScope.IsCallFromDao)
            {
                this._configScope.NodeContext = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig"), this._configScope.XmlNamespaceManager);
                this.ParseGlobalProperties();
            }
            this._configScope.ErrorContext.Activity = "loading global settings";
            XmlNodeList list = this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/settings/setting"), this._configScope.XmlNamespaceManager);
            if (list != null)
            {
                foreach (XmlNode node in list)
                {
                    if (node.Attributes["useStatementNamespaces"] != null)
                    {
                        string str = NodeUtils.ParsePropertyTokens(node.Attributes["useStatementNamespaces"].Value, this._configScope.Properties);
                        this._configScope.UseStatementNamespaces = Convert.ToBoolean(str);
                    }
                    if (node.Attributes["cacheModelsEnabled"] != null)
                    {
                        string str2 = NodeUtils.ParsePropertyTokens(node.Attributes["cacheModelsEnabled"].Value, this._configScope.Properties);
                        this._configScope.IsCacheModelsEnabled = Convert.ToBoolean(str2);
                    }
                    if (node.Attributes["useReflectionOptimizer"] != null)
                    {
                        string str3 = NodeUtils.ParsePropertyTokens(node.Attributes["useReflectionOptimizer"].Value, this._configScope.Properties);
                        this._configScope.UseReflectionOptimizer = Convert.ToBoolean(str3);
                    }
                    if (node.Attributes["validateSqlMap"] != null)
                    {
                        string str4 = NodeUtils.ParsePropertyTokens(node.Attributes["validateSqlMap"].Value, this._configScope.Properties);
                        this._configScope.ValidateSqlMap = Convert.ToBoolean(str4);
                    }
                }
            }
            if (this._objectFactory == null)
            {
                this._objectFactory = new IBatisNet.Common.Utilities.Objects.ObjectFactory(this._configScope.UseReflectionOptimizer);
            }
            if (this._setAccessorFactory == null)
            {
                this._setAccessorFactory = new IBatisNet.Common.Utilities.Objects.Members.SetAccessorFactory(this._configScope.UseReflectionOptimizer);
            }
            if (this._getAccessorFactory == null)
            {
                this._getAccessorFactory = new IBatisNet.Common.Utilities.Objects.Members.GetAccessorFactory(this._configScope.UseReflectionOptimizer);
            }
            if (this._sqlMapper == null)
            {
                AccessorFactory accessorFactory = new AccessorFactory(this._setAccessorFactory, this._getAccessorFactory);
                this._configScope.SqlMapper = new IBatisNet.DataMapper.SqlMapper(this._objectFactory, accessorFactory);
            }
            else
            {
                this._configScope.SqlMapper = this._sqlMapper;
            }
            ParameterMap parameterMap = new ParameterMap(this._configScope.DataExchangeFactory) {
                Id = "iBATIS.Empty.ParameterMap"
            };
            this._configScope.SqlMapper.AddParameterMap(parameterMap);
            this._configScope.SqlMapper.IsCacheModelsEnabled = this._configScope.IsCacheModelsEnabled;
            TypeAlias typeAlias = new TypeAlias(typeof(MemoryCacheControler)) {
                Name = "MEMORY"
            };
            this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
            typeAlias = new TypeAlias(typeof(LruCacheController)) {
                Name = "LRU"
            };
            this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
            typeAlias = new TypeAlias(typeof(FifoCacheController)) {
                Name = "FIFO"
            };
            this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
            typeAlias = new TypeAlias(typeof(AnsiStringTypeHandler)) {
                Name = "AnsiStringTypeHandler"
            };
            this._configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
            if (!this._configScope.IsCallFromDao)
            {
                this.GetProviders();
            }
            IDbProvider provider = null;
            if (!this._configScope.IsCallFromDao)
            {
                provider = this.ParseProvider();
                this._configScope.ErrorContext.Reset();
            }
            this._configScope.ErrorContext.Activity = "loading Database DataSource";
            XmlNode node2 = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/database/dataSource"), this._configScope.XmlNamespaceManager);
            if (node2 == null)
            {
                if (!this._configScope.IsCallFromDao)
                {
                    throw new ConfigurationException("There's no dataSource tag in SqlMap.config.");
                }
                this._configScope.SqlMapper.DataSource = this._configScope.DataSource;
            }
            else
            {
                if (!this._configScope.IsCallFromDao)
                {
                    this._configScope.ErrorContext.Resource = node2.OuterXml.ToString();
                    this._configScope.ErrorContext.MoreInfo = "parse DataSource";
                    DataSource source = DataSourceDeSerializer.Deserialize(node2);
                    source.DbProvider = provider;
                    source.ConnectionString = NodeUtils.ParsePropertyTokens(source.ConnectionString, this._configScope.Properties);
                    this._configScope.DataSource = source;
                    this._configScope.SqlMapper.DataSource = this._configScope.DataSource;
                }
                else
                {
                    this._configScope.SqlMapper.DataSource = this._configScope.DataSource;
                }
                this._configScope.ErrorContext.Reset();
            }
            foreach (XmlNode node3 in this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/alias/typeAlias"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.ErrorContext.Activity = "loading global Type alias";
                TypeAliasDeSerializer.Deserialize(node3, this._configScope);
            }
            this._configScope.ErrorContext.Reset();
            foreach (XmlNode node4 in this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/typeHandlers/typeHandler"), this._configScope.XmlNamespaceManager))
            {
                try
                {
                    this._configScope.ErrorContext.Activity = "loading typeHandler";
                    TypeHandlerDeSerializer.Deserialize(node4, this._configScope);
                }
                catch (Exception exception)
                {
                    NameValueCollection attributes = NodeUtils.ParseAttributes(node4, this._configScope.Properties);
                    throw new ConfigurationException(string.Format("Error registering TypeHandler class \"{0}\" for handling .Net type \"{1}\" and dbType \"{2}\". Cause: {3}", new object[] { NodeUtils.GetStringAttribute(attributes, "callback"), NodeUtils.GetStringAttribute(attributes, "type"), NodeUtils.GetStringAttribute(attributes, "dbType"), exception.Message }), exception);
                }
            }
            this._configScope.ErrorContext.Reset();
            foreach (XmlNode node5 in this._configScope.SqlMapConfigDocument.SelectNodes(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/sqlMaps/sqlMap"), this._configScope.XmlNamespaceManager))
            {
                this._configScope.NodeContext = node5;
                this.ConfigureSqlMap();
            }
            if (this._configScope.IsCacheModelsEnabled)
            {
                foreach (DictionaryEntry entry in this._configScope.SqlMapper.MappedStatements)
                {
                    this._configScope.ErrorContext.Activity = "Set CacheModel to statement";
                    IMappedStatement statement = (IMappedStatement) entry.Value;
                    if (statement.Statement.CacheModelName.Length > 0)
                    {
                        this._configScope.ErrorContext.MoreInfo = "statement : " + statement.Statement.Id;
                        this._configScope.ErrorContext.Resource = "cacheModel : " + statement.Statement.CacheModelName;
                        statement.Statement.CacheModel = this._configScope.SqlMapper.GetCache(statement.Statement.CacheModelName);
                    }
                }
            }
            this._configScope.ErrorContext.Reset();
            foreach (DictionaryEntry entry2 in this._configScope.CacheModelFlushOnExecuteStatements)
            {
                string key = (string) entry2.Key;
                IList list2 = (IList) entry2.Value;
                if ((list2 != null) && (list2.Count > 0))
                {
                    foreach (string str6 in list2)
                    {
                        IMappedStatement mappedStatement = this._configScope.SqlMapper.MappedStatements[str6] as IMappedStatement;
                        if (mappedStatement != null)
                        {
                            CacheModel cache = this._configScope.SqlMapper.GetCache(key);
                            if (_logger.IsDebugEnabled)
                            {
                                _logger.Debug("Registering trigger statement [" + mappedStatement.Id + "] to cache model [" + cache.Id + "]");
                            }
                            cache.RegisterTriggerStatement(mappedStatement);
                        }
                        else if (_logger.IsWarnEnabled)
                        {
                            _logger.Warn("Unable to register trigger statement [" + str6 + "] to cache model [" + key + "]. Statement does not exist.");
                        }
                    }
                }
            }
            foreach (DictionaryEntry entry3 in this._configScope.SqlMapper.ResultMaps)
            {
                this._configScope.ErrorContext.Activity = "Resolve 'resultMap' attribute on Result Property";
                ResultMap map2 = (ResultMap) entry3.Value;
                for (int i = 0; i < map2.Properties.Count; i++)
                {
                    ResultProperty mapping = map2.Properties[i];
                    if (mapping.NestedResultMapName.Length > 0)
                    {
                        mapping.NestedResultMap = this._configScope.SqlMapper.GetResultMap(mapping.NestedResultMapName);
                    }
                    mapping.PropertyStrategy = PropertyStrategyFactory.Get(mapping);
                }
                for (int j = 0; j < map2.Parameters.Count; j++)
                {
                    ResultProperty property2 = map2.Parameters[j];
                    if (property2.NestedResultMapName.Length > 0)
                    {
                        property2.NestedResultMap = this._configScope.SqlMapper.GetResultMap(property2.NestedResultMapName);
                    }
                    property2.ArgumentStrategy = ArgumentStrategyFactory.Get((ArgumentProperty) property2);
                }
                if (map2.Discriminator != null)
                {
                    map2.Discriminator.Initialize(this._configScope);
                }
            }
            this._configScope.ErrorContext.Reset();
        }

        public static void OnConfigFileChange(object obj)
        {
            StateConfig config = (StateConfig) obj;
            config.ConfigureHandler(null);
        }

        private bool ParseDynamicTags(XmlNode commandTextNode, IDynamicParent dynamic, StringBuilder sqlBuffer, bool isDynamic, bool postParseRequired, IStatement statement)
        {
            XmlNodeList childNodes = commandTextNode.ChildNodes;
            int count = childNodes.Count;
            for (int i = 0; i < count; i++)
            {
                XmlNode node = childNodes[i];
                if ((node.NodeType == XmlNodeType.CDATA) || (node.NodeType == XmlNodeType.Text))
                {
                    SqlText text;
                    string sqlStatement = NodeUtils.ParsePropertyTokens(node.InnerText.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' '), this._configScope.Properties);
                    if (postParseRequired)
                    {
                        text = new SqlText {
                            Text = sqlStatement.ToString()
                        };
                    }
                    else
                    {
                        text = this._paramParser.ParseInlineParameterMap(this._configScope, null, sqlStatement);
                    }
                    dynamic.AddChild(text);
                    sqlBuffer.Append(sqlStatement);
                }
                else if (node.Name == "include")
                {
                    string stringAttribute = NodeUtils.GetStringAttribute(NodeUtils.ParseAttributes(node, this._configScope.Properties), "refid");
                    XmlNode node2 = (XmlNode) this._configScope.SqlIncludes[stringAttribute];
                    if (node2 == null)
                    {
                        string str3 = this._configScope.ApplyNamespace(stringAttribute);
                        node2 = (XmlNode) this._configScope.SqlIncludes[str3];
                        if (node2 == null)
                        {
                            throw new ConfigurationException("Could not find SQL tag to include with refid '" + stringAttribute + "'");
                        }
                    }
                    isDynamic = this.ParseDynamicTags(node2, dynamic, sqlBuffer, isDynamic, false, statement);
                }
                else
                {
                    string name = node.Name;
                    IDeSerializer deSerializer = this._deSerializerFactory.GetDeSerializer(name);
                    if (deSerializer != null)
                    {
                        isDynamic = true;
                        SqlTag child = deSerializer.Deserialize(node);
                        dynamic.AddChild(child);
                        if (node.HasChildNodes)
                        {
                            isDynamic = this.ParseDynamicTags(node, child, sqlBuffer, isDynamic, child.Handler.IsPostParseRequired, statement);
                        }
                    }
                }
            }
            return isDynamic;
        }

        private void ParseGlobalProperties()
        {
            XmlNode node = this._configScope.NodeContext.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("properties"), this._configScope.XmlNamespaceManager);
            this._configScope.ErrorContext.Activity = "loading global properties";
            if (node != null)
            {
                if (node.HasChildNodes)
                {
                    foreach (XmlNode node2 in node.SelectNodes(this.ApplyDataMapperNamespacePrefix("property"), this._configScope.XmlNamespaceManager))
                    {
                        XmlAttribute attribute = node2.Attributes["key"];
                        XmlAttribute attribute2 = node2.Attributes["value"];
                        if ((attribute != null) && (attribute2 != null))
                        {
                            this._configScope.Properties.Add(attribute.Value, attribute2.Value);
                            if (_logger.IsDebugEnabled)
                            {
                                _logger.Debug(string.Format("Add property \"{0}\" value \"{1}\"", attribute.Value, attribute2.Value));
                            }
                        }
                        else
                        {
                            foreach (XmlNode node3 in Resources.GetAsXmlDocument(node2, this._configScope.Properties).SelectNodes("*/add", this._configScope.XmlNamespaceManager))
                            {
                                this._configScope.Properties[node3.Attributes["key"].Value] = node3.Attributes["value"].Value;
                                if (_logger.IsDebugEnabled)
                                {
                                    _logger.Debug(string.Format("Add property \"{0}\" value \"{1}\"", node3.Attributes["key"].Value, node3.Attributes["value"].Value));
                                }
                            }
                        }
                    }
                }
                else
                {
                    this._configScope.ErrorContext.Resource = node.OuterXml.ToString();
                    foreach (XmlNode node4 in Resources.GetAsXmlDocument(node, this._configScope.Properties).SelectNodes("/*/add"))
                    {
                        this._configScope.Properties[node4.Attributes["key"].Value] = node4.Attributes["value"].Value;
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.Debug(string.Format("Add property \"{0}\" value \"{1}\"", node4.Attributes["key"].Value, node4.Attributes["value"].Value));
                        }
                    }
                }
            }
            this._configScope.ErrorContext.Reset();
        }

        private IDbProvider ParseProvider()
        {
            this._configScope.ErrorContext.Activity = "load DataBase Provider";
            XmlNode node = this._configScope.SqlMapConfigDocument.SelectSingleNode(this.ApplyDataMapperNamespacePrefix("sqlMapConfig/database/provider"), this._configScope.XmlNamespaceManager);
            if (node != null)
            {
                this._configScope.ErrorContext.Resource = node.OuterXml.ToString();
                string key = NodeUtils.ParsePropertyTokens(node.Attributes["name"].Value, this._configScope.Properties);
                this._configScope.ErrorContext.ObjectId = key;
                if (!this._configScope.Providers.Contains(key))
                {
                    throw new ConfigurationException(string.Format("Error while configuring the Provider named \"{0}\". Cause : The provider is not in 'providers.config' or is not enabled.", key));
                }
                return (IDbProvider) this._configScope.Providers[key];
            }
            if (!this._configScope.Providers.Contains("_DEFAULT_PROVIDER_NAME"))
            {
                throw new ConfigurationException(string.Format("Error while configuring the SqlMap. There is no provider marked default in 'providers.config' file.", new object[0]));
            }
            return (IDbProvider) this._configScope.Providers["_DEFAULT_PROVIDER_NAME"];
        }

        private void ProcessSqlStatement(IStatement statement)
        {
            bool isDynamic = false;
            XmlNode nodeContext = this._configScope.NodeContext;
            DynamicSql dynamic = new DynamicSql(this._configScope, statement);
            StringBuilder sqlBuffer = new StringBuilder();
            if (statement.Id == "DynamicJIRA")
            {
                Console.Write("tt");
            }
            this._configScope.ErrorContext.MoreInfo = "process the Sql statement";
            if (statement.ExtendStatement.Length > 0)
            {
                XmlNode node2 = this._configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix("sqlMap/statements") + "/child::*[@id='" + statement.ExtendStatement + "']", this._configScope.XmlNamespaceManager);
                if (node2 == null)
                {
                    throw new ConfigurationException("Unable to find extend statement named '" + statement.ExtendStatement + "' on statement '" + statement.Id + "'.'");
                }
                nodeContext.InnerXml = node2.InnerXml + nodeContext.InnerXml;
            }
            this._configScope.ErrorContext.MoreInfo = "parse dynamic tags on sql statement";
            if (this.ParseDynamicTags(nodeContext, dynamic, sqlBuffer, isDynamic, false, statement))
            {
                statement.Sql = dynamic;
            }
            else
            {
                string sqlStatement = sqlBuffer.ToString();
                this.ApplyInlineParemeterMap(statement, sqlStatement);
            }
        }

        private void Reset()
        {
        }

        private void ValidateSchema(XmlNode section, string schemaFileName)
        {
            XmlReader reader = null;
            Stream stream = null;
            this._configScope.ErrorContext.Activity = "Validate SqlMap config";
            try
            {
                stream = this.GetStream(schemaFileName);
                if (stream == null)
                {
                    throw new ConfigurationException("Unable to locate embedded resource [IBatisNet.DataMapper." + schemaFileName + "]. If you are building from source, verfiy the file is marked as an embedded resource.");
                }
                XmlSchema schema = XmlSchema.Read(stream, new ValidationEventHandler(this.ValidationCallBack));
                XmlReaderSettings settings = new XmlReaderSettings {
                    ValidationType = ValidationType.Schema
                };
                XmlSchemaSet set = new XmlSchemaSet();
                set.Add(schema);
                settings.Schemas = set;
                reader = XmlReader.Create(new XmlNodeReader(section), settings);
                settings.ValidationEventHandler += new ValidationEventHandler(this.ValidationCallBack);
                while (reader.Read())
                {
                }
                if (!this._configScope.IsXmlValid)
                {
                    throw new ConfigurationException("Invalid SqlMap.config document. cause :" + this._configScope.ErrorContext.Resource);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            this._configScope.IsXmlValid = false;
            ErrorContext errorContext = this._configScope.ErrorContext;
            errorContext.Resource = errorContext.Resource + args.Message + Environment.NewLine;
        }

        public IGetAccessorFactory GetAccessorFactory
        {
            set
            {
                this._getAccessorFactory = value;
            }
        }

        public IObjectFactory ObjectFactory
        {
            set
            {
                this._objectFactory = value;
            }
        }

        public NameValueCollection Properties
        {
            set
            {
                this._configScope.Properties.Add(value);
            }
        }

        public ISetAccessorFactory SetAccessorFactory
        {
            set
            {
                this._setAccessorFactory = value;
            }
        }

        public ISqlMapper SqlMapper
        {
            set
            {
                this._sqlMapper = value;
            }
        }

        public bool ValidateSqlMapConfig
        {
            set
            {
                this._validateSqlMapConfig = value;
            }
        }
    }
}

