namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;

    public interface IPostSelectStrategy
    {
        void Execute(PostBindind postSelect, RequestScope request);
    }
}

