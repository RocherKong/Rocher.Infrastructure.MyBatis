namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public sealed class EnumTypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return Enum.Parse(parameterType, outputValue.ToString());
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex))
            {
                return DBNull.Value;
            }
            return Enum.Parse(mapping.MemberType, dataReader.GetValue(mapping.ColumnIndex).ToString());
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return Enum.Parse(mapping.MemberType, dataReader.GetValue(ordinal).ToString());
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            if (parameterValue != null)
            {
                dataParameter.Value = Convert.ChangeType(parameterValue, Enum.GetUnderlyingType(parameterValue.GetType()));
            }
            else
            {
                dataParameter.Value = DBNull.Value;
            }
        }

        public override object ValueOf(Type type, string s)
        {
            return Enum.Parse(type, s);
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

