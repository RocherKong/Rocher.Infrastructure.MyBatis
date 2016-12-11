namespace IBatisNet.DataMapper.MappedStatements.PropertStrategy
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class GroupByStrategy : BaseStrategy, IPropertyStrategy
    {
        private static IPropertyStrategy _resultMapStrategy = new ResultMapStrategy();

        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            IList list = null;
            object obj2 = ObjectProbe.GetMemberValue(target, mapping.PropertyName, request.DataExchangeFactory.AccessorFactory);
            if (obj2 == null)
            {
                obj2 = mapping.ListFactory.CreateInstance(null);
                mapping.SetAccessor.Set(target, obj2);
            }
            list = (IList) obj2;
            object sKIP = null;
            IResultMap map = mapping.NestedResultMap.ResolveSubMap(reader);
            if (map.GroupByProperties.Count > 0)
            {
                string key = base.GetUniqueKey(map, request, reader);
                IDictionary uniqueKeys = request.GetUniqueKeys(map);
                if ((uniqueKeys != null) && uniqueKeys.Contains(key))
                {
                    sKIP = uniqueKeys[key];
                    if (sKIP != null)
                    {
                        for (int i = 0; i < map.Properties.Count; i++)
                        {
                            ResultProperty property = map.Properties[i];
                            if (property.PropertyStrategy is GroupByStrategy)
                            {
                                property.PropertyStrategy.Set(request, map, property, ref sKIP, reader, null);
                            }
                        }
                    }
                    sKIP = BaseStrategy.SKIP;
                }
                else if (((key == null) || (uniqueKeys == null)) || !uniqueKeys.Contains(key))
                {
                    sKIP = _resultMapStrategy.Get(request, resultMap, mapping, ref target, reader);
                    if (uniqueKeys == null)
                    {
                        uniqueKeys = new Hashtable();
                        request.SetUniqueKeys(map, uniqueKeys);
                    }
                    uniqueKeys[key] = sKIP;
                }
            }
            else
            {
                sKIP = _resultMapStrategy.Get(request, resultMap, mapping, ref target, reader);
            }
            if ((sKIP != null) && (sKIP != BaseStrategy.SKIP))
            {
                list.Add(sKIP);
            }
            return sKIP;
        }

        public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            this.Get(request, resultMap, mapping, ref target, reader);
        }
    }
}

