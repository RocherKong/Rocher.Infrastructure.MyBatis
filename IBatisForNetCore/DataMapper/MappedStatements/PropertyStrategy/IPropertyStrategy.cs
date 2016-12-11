namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public interface IPropertyStrategy
    {
        object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader);
        void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object keys);
    }
}

