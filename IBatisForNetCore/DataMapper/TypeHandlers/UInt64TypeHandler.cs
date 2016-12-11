namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public sealed class UInt64TypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return Convert.ToUInt64(outputValue);
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex))
            {
                return DBNull.Value;
            }
            return Convert.ToUInt64(dataReader.GetValue(mapping.ColumnIndex));
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return Convert.ToUInt64(dataReader.GetValue(ordinal));
        }

        public override object ValueOf(Type type, string s)
        {
            return Convert.ToUInt64(s);
        }

        public override bool IsSimpleType
        {
            get
            {
                return true;
            }
        }
    }
}

