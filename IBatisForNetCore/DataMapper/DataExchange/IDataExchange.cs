namespace IBatisNet.DataMapper.DataExchange
{
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;

    public interface IDataExchange
    {
        object GetData(ParameterProperty mapping, object parameterObject);
        void SetData(ref object target, ParameterProperty mapping, object dataBaseValue);
        void SetData(ref object target, ResultProperty mapping, object dataBaseValue);
    }
}

