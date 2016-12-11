namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Commands;
    using IBatisNet.DataMapper.Configuration.Statements;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public interface IMappedStatement
    {
        event ExecuteEventHandler Execute;

        object ExecuteInsert(ISqlMapSession session, object parameterObject);
        IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty);
        IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate);
        IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject);
        IList ExecuteQueryForList(ISqlMapSession session, object parameterObject);
        void ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, IList<T> resultObject);
        void ExecuteQueryForList(ISqlMapSession session, object parameterObject, IList resultObject);
        IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, int skipResults, int maxResults);
        IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults);
        IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty);
        IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate);
        object ExecuteQueryForObject(ISqlMapSession session, object parameterObject);
        T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject);
        object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject);
        T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject, T resultObject);
        IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate);
        IList<T> ExecuteQueryForRowDelegate<T>(ISqlMapSession session, object parameterObject, RowDelegate<T> rowDelegate);
        int ExecuteUpdate(ISqlMapSession session, object parameterObject);

        string Id { get; }

        IPreparedCommand PreparedCommand { get; }

        ISqlMapper SqlMap { get; }

        IStatement Statement { get; }
    }
}

