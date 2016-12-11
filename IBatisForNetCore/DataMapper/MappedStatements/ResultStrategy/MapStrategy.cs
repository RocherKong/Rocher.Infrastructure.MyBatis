namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class MapStrategy : IResultStrategy
    {
        private static IResultStrategy _groupByStrategy = new GroupByStrategy();
        private static IResultStrategy _resultMapStrategy = new ResultMapStrategy();

        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            if (request.CurrentResultMap.ResolveSubMap(reader).GroupByPropertyNames.Count > 0)
            {
                return _groupByStrategy.Process(request, ref reader, resultObject);
            }
            return _resultMapStrategy.Process(request, ref reader, resultObject);
        }
    }
}

