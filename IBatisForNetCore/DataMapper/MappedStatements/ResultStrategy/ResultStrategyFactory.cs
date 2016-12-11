namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Configuration.Statements;
    using System;

    public sealed class ResultStrategyFactory
    {
        private static IResultStrategy _mapStrategy = new MapStrategy();
        private static IResultStrategy _objectStrategy = new ObjectStrategy();
        private static IResultStrategy _resultClassStrategy = new ResultClassStrategy();

        public static IResultStrategy Get(IStatement statement)
        {
            if (statement.ResultsMap.Count <= 0)
            {
                return _objectStrategy;
            }
            if (statement.ResultsMap[0] is ResultMap)
            {
                return _mapStrategy;
            }
            return _resultClassStrategy;
        }
    }
}

