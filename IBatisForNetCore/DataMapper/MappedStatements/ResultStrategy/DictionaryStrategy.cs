namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class DictionaryStrategy : IResultStrategy
    {
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object obj2 = resultObject;
            AutoResultMap currentResultMap = request.CurrentResultMap as AutoResultMap;
            if (obj2 == null)
            {
                obj2 = currentResultMap.CreateInstanceOfResultClass();
            }
            int fieldCount = reader.FieldCount;
            IDictionary dictionary = (IDictionary) obj2;
            for (int i = 0; i < fieldCount; i++)
            {
                ResultProperty property = new ResultProperty {
                    PropertyName = "value",
                    ColumnIndex = i,
                    TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(i))
                };
                dictionary.Add(reader.GetName(i), property.GetDataBaseValue(reader));
            }
            return obj2;
        }
    }
}

