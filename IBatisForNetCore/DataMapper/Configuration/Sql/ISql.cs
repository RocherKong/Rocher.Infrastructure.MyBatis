namespace IBatisNet.DataMapper.Configuration.Sql
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;

    public interface ISql
    {
        RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session);
    }
}

