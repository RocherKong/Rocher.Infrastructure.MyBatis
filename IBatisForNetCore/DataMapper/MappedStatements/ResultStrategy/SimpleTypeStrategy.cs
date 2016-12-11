namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class SimpleTypeStrategy : IResultStrategy
    {
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object target = resultObject;
            AutoResultMap currentResultMap = request.CurrentResultMap as AutoResultMap;
            if (target == null)
            {
                target = currentResultMap.CreateInstanceOfResultClass();
            }
            if (!currentResultMap.IsInitalized)
            {
                lock (currentResultMap)
                {
                    if (!currentResultMap.IsInitalized)
                    {
                        ResultProperty property;
                        property = new ResultProperty {
                            PropertyName = "value",
                            ColumnIndex = 0,
                            TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(target.GetType()),
                            PropertyStrategy = PropertyStrategyFactory.Get(property)
                        };
                        currentResultMap.Properties.Add(property);
                        currentResultMap.DataExchange = request.DataExchangeFactory.GetDataExchangeForClass(typeof(int));
                        currentResultMap.IsInitalized = true;
                    }
                }
            }
            currentResultMap.Properties[0].PropertyStrategy.Set(request, currentResultMap, currentResultMap.Properties[0], ref target, reader, null);
            return target;
        }
    }
}

