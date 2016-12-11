namespace IBatisNet.DataMapper.MappedStatements.ResultStrategy
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public interface IResultStrategy
    {
        object Process(RequestScope request, ref IDataReader reader, object resultObject);
    }
}

