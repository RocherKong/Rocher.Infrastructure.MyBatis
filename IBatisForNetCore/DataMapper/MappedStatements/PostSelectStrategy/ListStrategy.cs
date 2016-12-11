namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;

    public sealed class ListStrategy : IPostSelectStrategy
    {
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            object obj2 = postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, obj2);
        }
    }
}

