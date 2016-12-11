namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public sealed class CharTypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return Convert.ToChar(outputValue);
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex))
            {
                return DBNull.Value;
            }
            return dataReader.GetString(mapping.ColumnIndex)[0];
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return dataReader.GetString(ordinal)[0];
        }

        public override object ValueOf(Type type, string s)
        {
            return Convert.ToChar(s);
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

