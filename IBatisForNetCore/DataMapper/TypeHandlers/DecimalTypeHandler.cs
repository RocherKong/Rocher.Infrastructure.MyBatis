namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;
    using System.Globalization;

    public sealed class DecimalTypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return Convert.ToDecimal(outputValue);
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex))
            {
                return DBNull.Value;
            }
            return dataReader.GetDecimal(mapping.ColumnIndex);
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return dataReader.GetDecimal(ordinal);
        }

        public override object ValueOf(Type type, string s)
        {
            CultureInfo provider = new CultureInfo("en-US");
            return decimal.Parse(s, provider);
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

