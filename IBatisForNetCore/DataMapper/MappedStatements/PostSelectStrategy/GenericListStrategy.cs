namespace IBatisNet.DataMapper.MappedStatements.PostSelectStrategy
{
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class GenericListStrategy : IPostSelectStrategy
    {
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            Type[] genericArguments = postSelect.ResultProperty.SetAccessor.MemberType.GetGenericArguments();
            typeof(IList<>).MakeGenericType(genericArguments);
            Type type2 = postSelect.ResultProperty.SetAccessor.MemberType.GetGenericArguments()[0];
            MethodInfo[] methods = postSelect.Statement.GetType().GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance);
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
            object[] parameters = new object[] { request.Session, postSelect.Keys };
            object obj2 = info3.Invoke(postSelect.Statement, parameters);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, obj2);
        }
    }
}

