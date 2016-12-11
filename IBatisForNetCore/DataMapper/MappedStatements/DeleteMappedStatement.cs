namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Collections;

    public sealed class DeleteMappedStatement : MappedStatement
    {
        internal DeleteMappedStatement(ISqlMapper sqlMap, IStatement statement) : base(sqlMap, statement)
        {
        }

        public override object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query insert.");
        }

        public override IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query for list.");
        }

        public override void ExecuteQueryForList(ISqlMapSession session, object parameterObject, IList resultObject)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query for list.");
        }

        public override IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query for list.");
        }

        public override IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query for map.");
        }

        public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query for object.");
        }

        public override object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query for object.");
        }

        public override IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
        {
            throw new DataMapperException("Delete statements cannot be executed as a query for row delegate.");
        }
    }
}

