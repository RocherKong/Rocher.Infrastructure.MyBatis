namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class ResultClassStrategy : IResultStrategy
    {
        private static IResultStrategy _autoMapStrategy = new AutoMapStrategy();
        private static IResultStrategy _dictionaryStrategy = new DictionaryStrategy();
        private static IResultStrategy _listStrategy = new ListStrategy();
        private static IResultStrategy _simpleTypeStrategy = new SimpleTypeStrategy();

        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            if (request.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(request.CurrentResultMap.Class))
            {
                return _simpleTypeStrategy.Process(request, ref reader, resultObject);
            }
            if (typeof(IDictionary).IsAssignableFrom(request.CurrentResultMap.Class))
            {
                return _dictionaryStrategy.Process(request, ref reader, resultObject);
            }
            if (typeof(IList).IsAssignableFrom(request.CurrentResultMap.Class))
            {
                return _listStrategy.Process(request, ref reader, resultObject);
            }
            return _autoMapStrategy.Process(request, ref reader, resultObject);
        }
    }
}

