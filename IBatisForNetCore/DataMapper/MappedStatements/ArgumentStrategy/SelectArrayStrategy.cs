namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Commands;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class SelectArrayStrategy : IArgumentStrategy
    {
        public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
        {
            IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
            reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
            IList list = mappedStatement.ExecuteQueryForList(request.Session, keys);
            Array array = Array.CreateInstance(mapping.MemberType.GetElementType(), list.Count);
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                array.SetValue(list[i], i);
            }
            return array;
        }
    }
}

