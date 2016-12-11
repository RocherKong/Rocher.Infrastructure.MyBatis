namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Data;

    public sealed class DefaultStrategy : IArgumentStrategy
    {
        public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
        {
            if ((mapping.TypeHandler == null) || (mapping.TypeHandler is UnknownTypeHandler))
            {
                lock (mapping)
                {
                    if ((mapping.TypeHandler == null) || (mapping.TypeHandler is UnknownTypeHandler))
                    {
                        int i = 0;
                        if (mapping.ColumnIndex == -999999)
                        {
                            i = reader.GetOrdinal(mapping.ColumnName);
                        }
                        else
                        {
                            i = mapping.ColumnIndex;
                        }
                        Type fieldType = reader.GetFieldType(i);
                        mapping.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(fieldType);
                    }
                }
            }
            object dataBaseValue = mapping.GetDataBaseValue(reader);
            request.IsRowDataFound = request.IsRowDataFound || (dataBaseValue != null);
            return dataBaseValue;
        }
    }
}

