namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Commands;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class SelectObjectStrategy : IArgumentStrategy
    {
        public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
        {
            IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
            reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
            return mappedStatement.ExecuteQueryForObject(request.Session, keys);
        }
    }
}

