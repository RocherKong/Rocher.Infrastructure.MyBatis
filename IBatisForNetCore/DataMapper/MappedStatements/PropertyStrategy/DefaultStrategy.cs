namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Data;

    public sealed class DefaultStrategy : IPropertyStrategy
    {
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
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

        public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            object dataBaseValue = this.Get(request, resultMap, mapping, ref target, reader);
            resultMap.SetValueOfProperty(ref target, mapping, dataBaseValue);
        }
    }
}

