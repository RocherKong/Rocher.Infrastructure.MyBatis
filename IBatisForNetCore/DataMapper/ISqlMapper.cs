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
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.SessionStore;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;

    public interface ISqlMapper
    {
        void AddCache(CacheModel cache);
        void AddMappedStatement(string key, IMappedStatement mappedStatement);
        void AddParameterMap(ParameterMap parameterMap);
        void AddResultMap(IResultMap resultMap);
        ISqlMapSession BeginTransaction();
        ISqlMapSession BeginTransaction(bool openConnection);
        ISqlMapSession BeginTransaction(IsolationLevel isolationLevel);
        ISqlMapSession BeginTransaction(string connectionString);
        ISqlMapSession BeginTransaction(bool openNewConnection, IsolationLevel isolationLevel);
        ISqlMapSession BeginTransaction(string connectionString, IsolationLevel isolationLevel);
        ISqlMapSession BeginTransaction(string connectionString, bool openNewConnection, IsolationLevel isolationLevel);
        void CloseConnection();
        void CommitTransaction();
        void CommitTransaction(bool closeConnection);
        ISqlMapSession CreateSqlMapSession();
        int Delete(string statementName, object parameterObject);
        void FlushCaches();
        CacheModel GetCache(string name);
        string GetDataCacheStats();
        IMappedStatement GetMappedStatement(string id);
        ParameterMap GetParameterMap(string name);
        IResultMap GetResultMap(string name);
        object Insert(string statementName, object parameterObject);
        ISqlMapSession OpenConnection();
        ISqlMapSession OpenConnection(string connectionString);
        IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty);
        IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty);
        IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty);
        IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty);
        IDictionary<K, V> QueryForDictionary<K, V>(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate);
        IList<T> QueryForList<T>(string statementName, object parameterObject);
        IList QueryForList(string statementName, object parameterObject);
        void QueryForList<T>(string statementName, object parameterObject, IList<T> resultObject);
        void QueryForList(string statementName, object parameterObject, IList resultObject);
        IList<T> QueryForList<T>(string statementName, object parameterObject, int skipResults, int maxResults);
        IList QueryForList(string statementName, object parameterObject, int skipResults, int maxResults);
        IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty);
        IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty, string valueProperty);
        IDictionary QueryForMapWithRowDelegate(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate);
        object QueryForObject(string statementName, object parameterObject);
        T QueryForObject<T>(string statementName, object parameterObject);
        object QueryForObject(string statementName, object parameterObject, object resultObject);
        T QueryForObject<T>(string statementName, object parameterObject, T instanceObject);
        [Obsolete("This method will be remove in future version.", false)]
        PaginatedList QueryForPaginatedList(string statementName, object parameterObject, int pageSize);
        IList QueryWithRowDelegate(string statementName, object parameterObject, RowDelegate rowDelegate);
        IList<T> QueryWithRowDelegate<T>(string statementName, object parameterObject, RowDelegate<T> rowDelegate);
        void RollBackTransaction();
        void RollBackTransaction(bool closeConnection);
        int Update(string statementName, object parameterObject);

        IBatisNet.Common.Utilities.Objects.Members.AccessorFactory AccessorFactory { get; }

        IBatisNet.DataMapper.DataExchange.DataExchangeFactory DataExchangeFactory { get; }

        IDataSource DataSource { get; set; }

        IBatisNet.Common.Utilities.DBHelperParameterCache DBHelperParameterCache { get; }

        string Id { get; }

        bool IsCacheModelsEnabled { get; set; }

        bool IsSessionStarted { get; }

        ISqlMapSession LocalSession { get; }

        HybridDictionary MappedStatements { get; }

        IObjectFactory ObjectFactory { get; }

        HybridDictionary ParameterMaps { get; }

        HybridDictionary ResultMaps { get; }

        ISessionStore SessionStore { set; }

        IBatisNet.DataMapper.TypeHandlers.TypeHandlerFactory TypeHandlerFactory { get; }
    }
}

