namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class ObjectStrategy : IResultStrategy
    {
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object obj2 = resultObject;
            if (reader.FieldCount == 1)
            {
                ResultProperty property = new ResultProperty {
                    PropertyName = "value",
                    ColumnIndex = 0,
                    TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(0))
                };
                return property.GetDataBaseValue(reader);
            }
            if (reader.FieldCount <= 1)
            {
                return obj2;
            }
            object[] objArray = new object[reader.FieldCount];
            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                objArray[i] = new ResultProperty { PropertyName = "value", ColumnIndex = i, TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(i)) }.GetDataBaseValue(reader);
            }
            return objArray;
        }
    }
}

