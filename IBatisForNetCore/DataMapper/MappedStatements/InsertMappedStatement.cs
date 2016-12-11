namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Collections;

    public sealed class InsertMappedStatement : MappedStatement
    {
        internal InsertMappedStatement(ISqlMapper sqlMap, IStatement statement) : base(sqlMap, statement)
        {
        }

        public override IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for list.");
        }

        public override void ExecuteQueryForList(ISqlMapSession session, object parameterObject, IList resultObject)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for list.");
        }

        public override IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for list.");
        }

        public override IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for map.");
        }

        public override IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for row delegate.");
        }

        public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for object.");
        }

        public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for object.");
        }

        public override IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
        {
            throw new DataMapperException("Insert statements cannot be executed as a query for row delegate.");
        }

        public override int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Insert statements cannot be executed as a update query.");
        }
    }
}

