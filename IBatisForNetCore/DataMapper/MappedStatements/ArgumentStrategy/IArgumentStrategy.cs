namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public interface IArgumentStrategy
    {
        object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object keys);
    }
}

