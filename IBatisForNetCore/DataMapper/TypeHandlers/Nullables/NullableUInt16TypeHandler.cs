namespace IBatisNet.DataMapper.TypeHandlers.Nullables
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Data;

    public sealed class NullableUInt16TypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return new ushort?(Convert.ToUInt16(outputValue));
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex))
            {
                return DBNull.Value;
            }
            return new ushort?(Convert.ToUInt16(dataReader.GetValue(mapping.ColumnIndex)));
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return new ushort?(Convert.ToUInt16(dataReader.GetValue(ordinal)));
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            ushort? nullable = (ushort?) parameterValue;
            if (nullable.HasValue)
            {
                dataParameter.Value = nullable.Value;
            }
            else
            {
                dataParameter.Value = DBNull.Value;
            }
        }

        public override object ValueOf(Type type, string s)
        {
            return new ushort?(ushort.Parse(s));
        }

        public override bool IsSimpleType
        {
            get
            {
                return true;
            }
        }

        public override object NullValue
        {
            get
            {
                return null;
            }
        }
    }
}

