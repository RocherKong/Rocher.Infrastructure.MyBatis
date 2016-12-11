namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;

    public sealed class StrongTypedListStrategy : IPostSelectStrategy
    {
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            object obj2 = request.DataExchangeFactory.ObjectFactory.CreateFactory(postSelect.ResultProperty.SetAccessor.MemberType, Type.EmptyTypes).CreateInstance(null);
            postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys, (IList) obj2);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, obj2);
        }
    }
}

