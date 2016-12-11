namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class SelectArrayStrategy : IPropertyStrategy
    {
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            throw new NotSupportedException("Get method on ResultMapStrategy is not supported");
        }

        public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
            PostBindind bindind = new PostBindind {
                Statement = mappedStatement,
                Keys = keys,
                Target = target,
                ResultProperty = mapping
            };
            if (mapping.IsLazyLoad)
            {
                throw new NotImplementedException("Lazy load no supported for System.Array property:" + mapping.SetAccessor.Name);
            }
            bindind.Method = PostBindind.ExecuteMethod.ExecuteQueryForArrayList;
            request.QueueSelect.Enqueue(bindind);
        }
    }
}

