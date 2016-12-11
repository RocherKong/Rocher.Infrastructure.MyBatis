namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public sealed class ResultMapStrategy : BaseStrategy, IResultStrategy
    {
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object target = resultObject;
            IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);
            if (target == null)
            {
                object[] parameters = null;
                if (resultMap.Parameters.Count > 0)
                {
                    parameters = new object[resultMap.Parameters.Count];
                    for (int j = 0; j < resultMap.Parameters.Count; j++)
                    {
                        ResultProperty mapping = resultMap.Parameters[j];
                        parameters[j] = mapping.ArgumentStrategy.GetValue(request, mapping, ref reader, null);
                    }
                }
                target = resultMap.CreateInstanceOfResult(parameters);
            }
            for (int i = 0; i < resultMap.Properties.Count; i++)
            {
                ResultProperty property2 = resultMap.Properties[i];
                property2.PropertyStrategy.Set(request, resultMap, property2, ref target, reader, null);
            }
            return target;
        }
    }
}

