namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class ResultMapStrategy : BaseStrategy, IArgumentStrategy
    {
        public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
        {
            object[] parameters = null;
            bool flag = false;
            IResultMap resultMap = mapping.NestedResultMap.ResolveSubMap(reader);
            if (resultMap.Parameters.Count > 0)
            {
                parameters = new object[resultMap.Parameters.Count];
                for (int i = 0; i < resultMap.Parameters.Count; i++)
                {
                    ResultProperty property = resultMap.Parameters[i];
                    parameters[i] = property.ArgumentStrategy.GetValue(request, property, ref reader, null);
                    request.IsRowDataFound = request.IsRowDataFound || (parameters[i] != null);
                    flag = flag || (parameters[i] != null);
                }
            }
            object resultObject = null;
            if ((resultMap.Parameters.Count > 0) && !flag)
            {
                return null;
            }
            resultObject = resultMap.CreateInstanceOfResult(parameters);
            if (!base.FillObjectWithReaderAndResultMap(request, reader, resultMap, ref resultObject))
            {
                resultObject = null;
            }
            return resultObject;
        }
    }
}

