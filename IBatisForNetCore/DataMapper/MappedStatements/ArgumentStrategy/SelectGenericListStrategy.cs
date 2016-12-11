namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Commands;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    public sealed class SelectGenericListStrategy : IArgumentStrategy
    {
        public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys)
        {
            IMappedStatement mappedStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);
            reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
            Type[] genericArguments = mapping.MemberType.GetGenericArguments();
            typeof(IList<>).MakeGenericType(genericArguments);
            Type type2 = mapping.MemberType.GetGenericArguments()[0];
            MethodInfo[] methods = mappedStatement.GetType().GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance);
            MethodInfo info = null;
            foreach (MethodInfo info2 in methods)
            {
                if ((info2.IsGenericMethod && (info2.Name == "ExecuteQueryForList")) && (info2.GetParameters().Length == 2))
                {
                    info = info2;
                    break;
                }
            }
            MethodInfo info3 = info.MakeGenericMethod(new Type[] { type2 });
            object[] parameters = new object[] { request.Session, keys };
            return info3.Invoke(mappedStatement, parameters);
        }
    }
}

