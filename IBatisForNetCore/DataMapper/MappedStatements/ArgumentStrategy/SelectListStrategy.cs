namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Commands;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class SelectListStrategy : IArgumentStrategy
    {
        public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
        {
            IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
            if (mapping.MemberType == typeof(IList))
            {
                reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
                return mappedStatement.ExecuteQueryForList(request.Session, keys);
            }
            reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
            object obj2 = request.DataExchangeFactory.ObjectFactory.CreateFactory(mapping.MemberType, Type.EmptyTypes).CreateInstance(null);
            mappedStatement.ExecuteQueryForList(request.Session, keys, (IList) obj2);
            return obj2;
        }
    }
}

