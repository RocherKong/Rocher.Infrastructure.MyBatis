namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;

    public sealed class ArgumentStrategyFactory
    {
        private static IArgumentStrategy _defaultStrategy = new DefaultStrategy();
        private static IArgumentStrategy _resultMapStrategy = new ResultMapStrategy();
        private static IArgumentStrategy _selectArrayStrategy = new SelectArrayStrategy();
        private static IArgumentStrategy _selectGenericListStrategy = new SelectGenericListStrategy();
        private static IArgumentStrategy _selectListStrategy = new SelectListStrategy();
        private static IArgumentStrategy _selectObjectStrategy = new SelectObjectStrategy();

        public static IArgumentStrategy Get(ArgumentProperty mapping)
        {
            if ((mapping.Select.Length == 0) && (mapping.NestedResultMap == null))
            {
                return _defaultStrategy;
            }
            if (mapping.NestedResultMap != null)
            {
                return _resultMapStrategy;
            }
            return new SelectStrategy(mapping, _selectArrayStrategy, _selectGenericListStrategy, _selectListStrategy, _selectObjectStrategy);
        }
    }
}

