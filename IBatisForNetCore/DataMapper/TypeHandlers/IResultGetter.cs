namespace IBatisNet.DataMapper.TypeHandlers
{
    using System;
    using System.Data;

    public interface IResultGetter
    {
        IDataReader DataReader { get; }

        object Value { get; }
    }
}

