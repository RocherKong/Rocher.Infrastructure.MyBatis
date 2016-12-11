namespace IBatisNet.DataMapper
{
    using IBatisNet.Common;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Cache;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.SessionStore;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Text;

    public class SqlMapper : ISqlMapper
    {
        private IBatisNet.Common.Utilities.Objects.Members.AccessorFactory _accessorFactory;
        private HybridDictionary _cacheMaps = new HybridDictionary();
        private bool _cacheModelsEnabled;
        private IBatisNet.DataMapper.DataExchange.DataExchangeFactory _dataExchangeFactory;
        private IDataSource _dataSource;
        private IBatisNet.Common.Utilities.DBHelperParameterCache _dbHelperParameterCache = new IBatisNet.Common.Utilities.DBHelperParameterCache();
        private string _id = string.Empty;
        private HybridDictionary _mappedStatements = new HybridDictionary();
        private IObjectFactory _objectFactory;
        private HybridDictionary _parameterMaps = new HybridDictionary();
        private HybridDictionary _resultMaps = new HybridDictionary();
        private ISessionStore _sessionStore;
        private IBatisNet.DataMapper.TypeHandlers.TypeHandlerFactory _typeHandlerFactory = new IBatisNet.DataMapper.TypeHandlers.TypeHandlerFactory();

        public SqlMapper(IObjectFactory objectFactory, IBatisNet.Common.Utilities.Objects.Members.AccessorFactory accessorFactory)
        {
            this._objectFactory = objectFactory;
            this._accessorFactory = accessorFactory;
            this._dataExchangeFactory = new IBatisNet.DataMapper.DataExchange.DataExchangeFactory(this._typeHandlerFactory, this._objectFactory, accessorFactory);
            this._id = HashCodeProvider.GetIdentityHashCode(this).ToString();
            this._sessionStore = SessionStoreFactory.GetSessionStore(this._id);
        }

        public void AddCache(CacheModel cache)
        {
            if (this._cacheMaps.Contains(cache.Id))
            {
                throw new DataMapperException("This SQL map already contains an Cache named " + cache.Id);
            }
            this._cacheMaps.Add(cache.Id, cache);
        }

        public void AddMappedStatement(string key, IMappedStatement mappedStatement)
        {
            if (this._mappedStatements.Contains(key))
            {
                throw new DataMapperException("This SQL map already contains a MappedStatement named " + mappedStatement.Id);
            }
            this._mappedStatements.Add(key, mappedStatement);
        }

        public void AddParameterMap(ParameterMap parameterMap)
        {
            if (this._parameterMaps.Contains(parameterMap.Id))
            {
                throw new DataMapperException("This SQL map already contains an ParameterMap named " + parameterMap.Id);
            }
            this._parameterMaps.Add(parameterMap.Id, parameterMap);
        }

        public void AddResultMap(IResultMap resultMap)
        {
            if (this._resultMaps.Contains(resultMap.Id))
            {
                throw new DataMapperException("This SQL map already contains an ResultMap named " + resultMap.Id);
            }
            this._resultMaps.Add(resultMap.Id, resultMap);
        }

        public ISqlMapSession BeginTransaction()
        {
            if (this._sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = this.CreateSqlMapSession();
            this._sessionStore.Store(session);
            session.BeginTransaction();
            return session;
        }

        public ISqlMapSession BeginTransaction(bool openConnection)
        {
            ISqlMapSession localSession = null;
            if (openConnection)
            {
                return this.BeginTransaction();
            }
            localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
            }
            localSession.BeginTransaction(openConnection);
            return localSession;
        }

        public ISqlMapSession BeginTransaction(IsolationLevel isolationLevel)
        {
            if (this._sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = this.CreateSqlMapSession();
            this._sessionStore.Store(session);
            session.BeginTransaction(isolationLevel);
            return session;
        }

        public ISqlMapSession BeginTransaction(string connectionString)
        {
            if (this._sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = this.CreateSqlMapSession(connectionString);
            this._sessionStore.Store(session);
            session.BeginTransaction(connectionString);
            return session;
        }

        public ISqlMapSession BeginTransaction(bool openNewConnection, IsolationLevel isolationLevel)
        {
            ISqlMapSession localSession = null;
            if (openNewConnection)
            {
                return this.BeginTransaction(isolationLevel);
            }
            localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
            }
            localSession.BeginTransaction(openNewConnection, isolationLevel);
            return localSession;
        }

        public ISqlMapSession BeginTransaction(string connectionString, IsolationLevel isolationLevel)
        {
            if (this._sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = this.CreateSqlMapSession(connectionString);
            this._sessionStore.Store(session);
            session.BeginTransaction(connectionString, isolationLevel);
            return session;
        }

        public ISqlMapSession BeginTransaction(string connectionString, bool openNewConnection, IsolationLevel isolationLevel)
        {
            ISqlMapSession localSession = null;
            if (openNewConnection)
            {
                return this.BeginTransaction(connectionString, isolationLevel);
            }
            localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
            }
            localSession.BeginTransaction(connectionString, openNewConnection, isolationLevel);
            return localSession;
        }

        public void CloseConnection()
        {
            if (this._sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke CloseConnection(). No connection was started. Call OpenConnection() first.");
            }
            try
            {
                this._sessionStore.LocalSession.CloseConnection();
            }
            catch (Exception exception)
            {
                throw new DataMapperException("SqlMapper could not CloseConnection(). Cause :" + exception.Message, exception);
            }
            finally
            {
                this._sessionStore.Dispose();
            }
        }

        public void CommitTransaction()
        {
            if (this._sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                this._sessionStore.LocalSession.CommitTransaction();
            }
            finally
            {
                this._sessionStore.Dispose();
            }
        }

        public void CommitTransaction(bool closeConnection)
        {
            if (this._sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                this._sessionStore.LocalSession.CommitTransaction(closeConnection);
            }
            finally
            {
                if (closeConnection)
                {
                    this._sessionStore.Dispose();
                }
            }
        }

        public ISqlMapSession CreateSqlMapSession()
        {
            ISqlMapSession session = new SqlMapSession(this);
            session.CreateConnection();
            return session;
        }

        public ISqlMapSession CreateSqlMapSession(string connectionString)
        {
            ISqlMapSession session = new SqlMapSession(this);
            session.CreateConnection(connectionString);
            return session;
        }

        public int Delete(string statementName, object parameterObject)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            int num = 0;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                num = this.GetMappedStatement(statementName).ExecuteUpdate(localSession, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return num;
        }

        public void FlushCaches()
        {
            IDictionaryEnumerator enumerator = this._cacheMaps.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ((CacheModel) enumerator.Value).Flush();
            }
        }

        public CacheModel GetCache(string name)
        {
            if (!this._cacheMaps.Contains(name))
            {
                throw new DataMapperException("This SQL map does not contain an Cache named " + name);
            }
            return (CacheModel) this._cacheMaps[name];
        }

        public string GetDataCacheStats()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Environment.NewLine);
            builder.Append("Cache Data Statistics");
            builder.Append(Environment.NewLine);
            builder.Append("=====================");
            builder.Append(Environment.NewLine);
            IDictionaryEnumerator enumerator = this._mappedStatements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IMappedStatement statement = (IMappedStatement) enumerator.Value;
                builder.Append(statement.Id);
                builder.Append(": ");
                if (statement is CachingStatement)
                {
                    double dataCacheHitRatio = ((CachingStatement) statement).GetDataCacheHitRatio();
                    if (dataCacheHitRatio != -1.0)
                    {
                        builder.Append(Math.Round((double) (dataCacheHitRatio * 100.0)));
                        builder.Append("%");
                    }
                    else
                    {
                        builder.Append("No Cache.");
                    }
                }
                else
                {
                    builder.Append("No Cache.");
                }
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }

        public IMappedStatement GetMappedStatement(string id)
        {
            if (!this._mappedStatements.Contains(id))
            {
                throw new DataMapperException("This SQL map does not contain a MappedStatement named " + id);
            }
            return (IMappedStatement) this._mappedStatements[id];
        }

        public ParameterMap GetParameterMap(string name)
        {
            if (!this._parameterMaps.Contains(name))
            {
                throw new DataMapperException("This SQL map does not contain an ParameterMap named " + name + ".  ");
            }
            return (ParameterMap) this._parameterMaps[name];
        }

        public IResultMap GetResultMap(string name)
        {
            if (!this._resultMaps.Contains(name))
            {
                throw new DataMapperException("This SQL map does not contain an ResultMap named " + name);
            }
            return (ResultMap) this._resultMaps[name];
        }

        public object Insert(string statementName, object parameterObject)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            object obj2 = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                obj2 = this.GetMappedStatement(statementName).ExecuteInsert(localSession, parameterObject);
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return obj2;
        }

        public ISqlMapSession OpenConnection()
        {
            if (this._sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke OpenConnection(). A connection is already started. Call CloseConnection first.");
            }
            ISqlMapSession session = this.CreateSqlMapSession();
            this._sessionStore.Store(session);
            return session;
        }

        public ISqlMapSession OpenConnection(string connectionString)
        {
            if (this._sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke OpenConnection(). A connection is already started. Call CloseConnection first.");
            }
            ISqlMapSession session = this.CreateSqlMapSession(connectionString);
            this._sessionStore.Store(session);
            return session;
        }

        public IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty)
        {
            return this.QueryForDictionary<K, V>(statementName, parameterObject, keyProperty, null);
        }

        public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty)
        {
            return this.QueryForMap(statementName, parameterObject, keyProperty);
        }

        public IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            IDictionary<K, V> dictionary = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                dictionary = this.GetMappedStatement(statementName).ExecuteQueryForDictionary<K, V>(localSession, parameterObject, keyProperty, valueProperty);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return dictionary;
        }

        public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty)
        {
            return this.QueryForMap(statementName, parameterObject, keyProperty, valueProperty);
        }

        public IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            IDictionary<K, V> dictionary = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                dictionary = this.GetMappedStatement(statementName).ExecuteQueryForDictionary<K, V>(localSession, parameterObject, keyProperty, valueProperty, rowDelegate);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return dictionary;
        }

        public IList<T> QueryForList<T>(string statementName, object parameterObject)
        {
            IList<T> list;
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                list = this.GetMappedStatement(statementName).ExecuteQueryForList<T>(localSession, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return list;
        }

        public IList QueryForList(string statementName, object parameterObject)
        {
            IList list;
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                list = this.GetMappedStatement(statementName).ExecuteQueryForList(localSession, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return list;
        }

        public void QueryForList<T>(string statementName, object parameterObject, IList<T> resultObject)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (resultObject == null)
            {
                throw new DataMapperException("resultObject parameter must be instantiated before being passed to SqlMapper.QueryForList");
            }
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                this.GetMappedStatement(statementName).ExecuteQueryForList<T>(localSession, parameterObject, resultObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
        }

        public void QueryForList(string statementName, object parameterObject, IList resultObject)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (resultObject == null)
            {
                throw new DataMapperException("resultObject parameter must be instantiated before being passed to SqlMapper.QueryForList");
            }
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                this.GetMappedStatement(statementName).ExecuteQueryForList(localSession, parameterObject, resultObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
        }

        public IList<T> QueryForList<T>(string statementName, object parameterObject, int skipResults, int maxResults)
        {
            IList<T> list;
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                list = this.GetMappedStatement(statementName).ExecuteQueryForList<T>(localSession, parameterObject, skipResults, maxResults);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return list;
        }

        public IList QueryForList(string statementName, object parameterObject, int skipResults, int maxResults)
        {
            IList list;
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                list = this.GetMappedStatement(statementName).ExecuteQueryForList(localSession, parameterObject, skipResults, maxResults);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return list;
        }

        public IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty)
        {
            return this.QueryForMap(statementName, parameterObject, keyProperty, null);
        }

        public IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty, string valueProperty)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            IDictionary dictionary = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                dictionary = this.GetMappedStatement(statementName).ExecuteQueryForMap(localSession, parameterObject, keyProperty, valueProperty);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return dictionary;
        }

        public IDictionary QueryForMapWithRowDelegate(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            IDictionary dictionary = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                dictionary = this.GetMappedStatement(statementName).ExecuteQueryForMapWithRowDelegate(localSession, parameterObject, keyProperty, valueProperty, rowDelegate);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return dictionary;
        }

        public object QueryForObject(string statementName, object parameterObject)
        {
            object obj2;
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                obj2 = this.GetMappedStatement(statementName).ExecuteQueryForObject(localSession, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return obj2;
        }

        public T QueryForObject<T>(string statementName, object parameterObject)
        {
            T local;
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                local = this.GetMappedStatement(statementName).ExecuteQueryForObject<T>(localSession, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return local;
        }

        public object QueryForObject(string statementName, object parameterObject, object resultObject)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            object obj2 = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                obj2 = this.GetMappedStatement(statementName).ExecuteQueryForObject(localSession, parameterObject, resultObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return obj2;
        }

        public T QueryForObject<T>(string statementName, object parameterObject, T instanceObject)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            T local = default(T);
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                local = this.GetMappedStatement(statementName).ExecuteQueryForObject<T>(localSession, parameterObject, instanceObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return local;
        }

        [Obsolete("This method will be remove in future version.", false)]
        public PaginatedList QueryForPaginatedList(string statementName, object parameterObject, int pageSize)
        {
            return new PaginatedList(this.GetMappedStatement(statementName), parameterObject, pageSize);
        }

        public IList QueryWithRowDelegate(string statementName, object parameterObject, RowDelegate rowDelegate)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            IList list = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                list = this.GetMappedStatement(statementName).ExecuteQueryForRowDelegate(localSession, parameterObject, rowDelegate);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return list;
        }

        public IList<T> QueryWithRowDelegate<T>(string statementName, object parameterObject, RowDelegate<T> rowDelegate)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            IList<T> list = null;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                list = this.GetMappedStatement(statementName).ExecuteQueryForRowDelegate<T>(localSession, parameterObject, rowDelegate);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return list;
        }

        public void RollBackTransaction()
        {
            if (this._sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke RollBackTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                this._sessionStore.LocalSession.RollBackTransaction();
            }
            finally
            {
                this._sessionStore.Dispose();
            }
        }

        public void RollBackTransaction(bool closeConnection)
        {
            if (this._sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke RollBackTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                this._sessionStore.LocalSession.RollBackTransaction(closeConnection);
            }
            finally
            {
                if (closeConnection)
                {
                    this._sessionStore.Dispose();
                }
            }
        }

        public int Update(string statementName, object parameterObject)
        {
            bool flag = false;
            ISqlMapSession localSession = this._sessionStore.LocalSession;
            int num = 0;
            if (localSession == null)
            {
                localSession = this.CreateSqlMapSession();
                flag = true;
            }
            try
            {
                num = this.GetMappedStatement(statementName).ExecuteUpdate(localSession, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (flag)
                {
                    localSession.CloseConnection();
                }
            }
            return num;
        }

        public IBatisNet.Common.Utilities.Objects.Members.AccessorFactory AccessorFactory
        {
            get
            {
                return this._accessorFactory;
            }
        }

        public IBatisNet.DataMapper.DataExchange.DataExchangeFactory DataExchangeFactory
        {
            get
            {
                return this._dataExchangeFactory;
            }
        }

        public IDataSource DataSource
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

        public IBatisNet.Common.Utilities.DBHelperParameterCache DBHelperParameterCache
        {
            get
            {
                return this._dbHelperParameterCache;
            }
        }

        public string Id
        {
            get
            {
                return this._id;
            }
        }

        public bool IsCacheModelsEnabled
        {
            get
            {
                return this._cacheModelsEnabled;
            }
            set
            {
                this._cacheModelsEnabled = value;
            }
        }

        public bool IsSessionStarted
        {
            get
            {
                return (this._sessionStore.LocalSession != null);
            }
        }

        public ISqlMapSession LocalSession
        {
            get
            {
                return this._sessionStore.LocalSession;
            }
        }

        public HybridDictionary MappedStatements
        {
            get
            {
                return this._mappedStatements;
            }
        }

        public IObjectFactory ObjectFactory
        {
            get
            {
                return this._objectFactory;
            }
        }

        public HybridDictionary ParameterMaps
        {
            get
            {
                return this._parameterMaps;
            }
        }

        public HybridDictionary ResultMaps
        {
            get
            {
                return this._resultMaps;
            }
        }

        public ISessionStore SessionStore
        {
            set
            {
                this._sessionStore = value;
            }
        }

        public IBatisNet.DataMapper.TypeHandlers.TypeHandlerFactory TypeHandlerFactory
        {
            get
            {
                return this._typeHandlerFactory;
            }
        }
    }
}

