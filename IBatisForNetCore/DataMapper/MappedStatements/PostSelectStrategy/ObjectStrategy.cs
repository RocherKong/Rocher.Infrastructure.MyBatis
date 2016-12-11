namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;

    public sealed class ObjectStrategy : IPostSelectStrategy
    {
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            object obj2 = postSelect.Statement.ExecuteQueryForObject(request.Session, postSelect.Keys);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, obj2);
        }
    }
}

