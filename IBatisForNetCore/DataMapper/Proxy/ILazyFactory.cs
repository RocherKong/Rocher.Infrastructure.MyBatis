namespace IBatisNet.DataMapper.Proxy
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.MappedStatements;
    using System;

    public interface ILazyFactory
    {
        object CreateProxy(IMappedStatement mappedStatement, object param, object target, ISetAccessor setAccessor);
    }
}

