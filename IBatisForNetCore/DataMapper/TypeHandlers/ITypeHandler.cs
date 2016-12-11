namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public interface ITypeHandler
    {
        bool Equals(object obj, string str);
        object GetDataBaseValue(object outputValue, Type parameterType);
        object GetValueByIndex(ResultProperty mapping, IDataReader dataReader);
        object GetValueByName(ResultProperty mapping, IDataReader dataReader);
        void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType);
        object ValueOf(Type type, string s);

        bool IsSimpleType { get; }

        object NullValue { get; }
    }
}

