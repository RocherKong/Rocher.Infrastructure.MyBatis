namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class ResultMapStrategy : BaseStrategy, IPropertyStrategy
    {
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            object[] parameters = null;
            bool flag = false;
            IResultMap map = mapping.NestedResultMap.ResolveSubMap(reader);
            if (map.Parameters.Count > 0)
            {
                parameters = new object[map.Parameters.Count];
                for (int i = 0; i < map.Parameters.Count; i++)
                {
                    ResultProperty property = map.Parameters[i];
                    parameters[i] = property.ArgumentStrategy.GetValue(request, property, ref reader, null);
                    request.IsRowDataFound = request.IsRowDataFound || (parameters[i] != null);
                    flag = flag || (parameters[i] != null);
                }
            }
            object resultObject = null;
            if ((map.Parameters.Count > 0) && !flag)
            {
                return null;
            }
            resultObject = map.CreateInstanceOfResult(parameters);
            if (!base.FillObjectWithReaderAndResultMap(request, reader, map, ref resultObject))
            {
                resultObject = null;
            }
            return resultObject;
        }

        public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            object dataBaseValue = this.Get(request, resultMap, mapping, ref target, reader);
            resultMap.SetValueOfProperty(ref target, mapping, dataBaseValue);
        }
    }
}

