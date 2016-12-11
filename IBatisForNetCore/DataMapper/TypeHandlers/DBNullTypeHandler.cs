namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public sealed class DBNullTypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return DBNull.Value;
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            return DBNull.Value;
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            return DBNull.Value;
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            dataParameter.Value = DBNull.Value;
        }

        public override object ValueOf(Type type, string s)
        {
            return DBNull.Value;
        }

        public override bool IsSimpleType
        {
            get
            {
                return false;
            }
        }
    }
}

