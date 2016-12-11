namespace IBatisNet.DataMapper.Proxy
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.MappedStatements;
    using System;

    public class LazyListGenericFactory : ILazyFactory
    {
        public object CreateProxy(IMappedStatement mappedStatement, object param, object target, ISetAccessor setAccessor)
        {
            Type type = setAccessor.MemberType.GetGenericArguments()[0];
            Type typeToCreate = typeof(LazyListGeneric<>).MakeGenericType(new Type[] { type });
            Type[] types = new Type[] { typeof(IMappedStatement), typeof(object), typeof(object), typeof(ISetAccessor) };
            IFactory factory = mappedStatement.SqlMap.DataExchangeFactory.ObjectFactory.CreateFactory(typeToCreate, types);
            object[] parameters = new object[] { mappedStatement, param, target, setAccessor };
            return factory.CreateInstance(parameters);
        }
    }
}

