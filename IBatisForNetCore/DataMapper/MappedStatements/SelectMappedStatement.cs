namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Exceptions;
    using System;

    public sealed class SelectMappedStatement : MappedStatement
    {
        internal SelectMappedStatement(ISqlMapper sqlMap, IStatement statement) : base(sqlMap, statement)
        {
        }

        public override object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Update statements cannot be executed as a query insert.");
        }

        public override int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Insert statements cannot be executed as a update query.");
        }
    }
}

