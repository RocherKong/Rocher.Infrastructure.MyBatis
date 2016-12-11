namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class AutoMapStrategy : IResultStrategy
    {
        private AutoResultMap InitializeAutoResultMap(RequestScope request, ref IDataReader reader, ref object resultObject)
        {
            AutoResultMap currentResultMap = request.CurrentResultMap as AutoResultMap;
            if (request.Statement.AllowRemapping)
            {
                currentResultMap = currentResultMap.Clone();
                ResultPropertyCollection propertys = ReaderAutoMapper.Build(request.DataExchangeFactory, reader, ref resultObject);
                currentResultMap.Properties.AddRange(propertys);
                return currentResultMap;
            }
            if (!currentResultMap.IsInitalized)
            {
                lock (currentResultMap)
                {
                    if (!currentResultMap.IsInitalized)
                    {
                        ResultPropertyCollection propertys2 = ReaderAutoMapper.Build(request.DataExchangeFactory, reader, ref resultObject);
                        currentResultMap.Properties.AddRange(propertys2);
                        currentResultMap.IsInitalized = true;
                    }
                }
            }
            return currentResultMap;
        }

        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object obj2 = resultObject;
            if (obj2 == null)
            {
                obj2 = (request.CurrentResultMap as AutoResultMap).CreateInstanceOfResultClass();
            }
            AutoResultMap map = this.InitializeAutoResultMap(request, ref reader, ref obj2);
            for (int i = 0; i < map.Properties.Count; i++)
            {
                ResultProperty property = map.Properties[i];
                map.SetValueOfProperty(ref obj2, property, property.GetDataBaseValue(reader));
            }
            return obj2;
        }
    }
}

