namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.MappedStatements.PropertStrategy;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class GroupByStrategy : BaseStrategy, IResultStrategy
    {
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object target = resultObject;
            IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);
            string key = base.GetUniqueKey(resultMap, request, reader);
            IDictionary uniqueKeys = request.GetUniqueKeys(resultMap);
            if ((uniqueKeys != null) && uniqueKeys.Contains(key))
            {
                target = uniqueKeys[key];
                for (int i = 0; i < resultMap.Properties.Count; i++)
                {
                    ResultProperty mapping = resultMap.Properties[i];
                    if (mapping.PropertyStrategy is IBatisNet.DataMapper.MappedStatements.PropertStrategy.GroupByStrategy)
                    {
                        mapping.PropertyStrategy.Set(request, resultMap, mapping, ref target, reader, null);
                    }
                }
                return BaseStrategy.SKIP;
            }
            if (((key == null) || (uniqueKeys == null)) || !uniqueKeys.Contains(key))
            {
                if (target == null)
                {
                    target = resultMap.CreateInstanceOfResult(null);
                }
                for (int j = 0; j < resultMap.Properties.Count; j++)
                {
                    ResultProperty property2 = resultMap.Properties[j];
                    property2.PropertyStrategy.Set(request, resultMap, property2, ref target, reader, null);
                }
                if (uniqueKeys == null)
                {
                    uniqueKeys = new Hashtable();
                    request.SetUniqueKeys(resultMap, uniqueKeys);
                }
                uniqueKeys[key] = target;
            }
            return target;
        }
    }
}

