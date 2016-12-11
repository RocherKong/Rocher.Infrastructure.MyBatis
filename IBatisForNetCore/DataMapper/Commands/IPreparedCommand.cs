namespace IBatisNet.DataMapper.Commands
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;

    public interface IPreparedCommand
    {
        void Create(RequestScope request, ISqlMapSession session, IStatement statement, object parameterObject);
    }
}

