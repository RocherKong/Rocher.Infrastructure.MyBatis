namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class SelectObjectStrategy : IPropertyStrategy
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
                object obj2 = mapping.LazyFactory.CreateProxy(mappedStatement, keys, target, mapping.SetAccessor);
                mapping.SetAccessor.Set(target, obj2);
            }
            else
            {
                bindind.Method = PostBindind.ExecuteMethod.ExecuteQueryForObject;
                request.QueueSelect.Enqueue(bindind);
            }
        }
    }
}

