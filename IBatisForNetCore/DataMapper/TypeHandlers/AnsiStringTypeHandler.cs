namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public class AnsiStringTypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return outputValue;
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex))
            {
                return DBNull.Value;
            }
            return dataReader.GetString(mapping.ColumnIndex);
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return dataReader.GetString(ordinal);
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            dataParameter.DbType = DbType.AnsiString;
            base.SetParameter(dataParameter, parameterValue, dbType);
        }

        public override object ValueOf(Type type, string s)
        {
            return s;
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

