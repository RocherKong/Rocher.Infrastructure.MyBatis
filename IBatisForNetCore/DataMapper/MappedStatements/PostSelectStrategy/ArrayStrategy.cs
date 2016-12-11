namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;

    public sealed class ArrayStrategy : IPostSelectStrategy
    {
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            IList list = postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys);
            Array array = Array.CreateInstance(postSelect.ResultProperty.SetAccessor.MemberType.GetElementType(), list.Count);
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                array.SetValue(list[i], i);
            }
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, array);
        }
    }
}

