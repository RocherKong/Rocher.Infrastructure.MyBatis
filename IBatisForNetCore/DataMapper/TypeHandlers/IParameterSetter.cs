namespace IBatisNet.DataMapper.TypeHandlers
{
    using System;
    using System.Data;

    public interface IParameterSetter
    {
        IDataParameter DataParameter { get; }

        object Value { set; }
    }
}

