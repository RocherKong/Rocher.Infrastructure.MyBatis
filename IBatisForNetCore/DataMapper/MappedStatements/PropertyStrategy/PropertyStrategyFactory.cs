namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class PropertyStrategyFactory
    {
        private static IPropertyStrategy _defaultStrategy = new DefaultStrategy();
        private static IPropertyStrategy _groupByStrategy = new GroupByStrategy();
        private static IPropertyStrategy _resultMapStrategy = new ResultMapStrategy();
        private static IPropertyStrategy _selectArrayStrategy = new SelectArrayStrategy();
        private static IPropertyStrategy _selectGenericListStrategy = new SelectGenericListStrategy();
        private static IPropertyStrategy _selectListStrategy = new SelectListStrategy();
        private static IPropertyStrategy _selectObjectStrategy = new SelectObjectStrategy();

        public static IPropertyStrategy Get(ResultProperty mapping)
        {
            if ((mapping.Select.Length == 0) && (mapping.NestedResultMap == null))
            {
                return _defaultStrategy;
            }
            if (mapping.NestedResultMap == null)
            {
                return new SelectStrategy(mapping, _selectArrayStrategy, _selectGenericListStrategy, _selectListStrategy, _selectObjectStrategy);
            }
            if (mapping.NestedResultMap.GroupByPropertyNames.Count > 0)
            {
                return _groupByStrategy;
            }
            if (mapping.MemberType.IsGenericType && typeof(IList<>).IsAssignableFrom(mapping.MemberType.GetGenericTypeDefinition()))
            {
                return _groupByStrategy;
            }
            if (typeof(IList).IsAssignableFrom(mapping.MemberType))
            {
                return _groupByStrategy;
            }
            return _resultMapStrategy;
        }
    }
}

