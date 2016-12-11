namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public sealed class TimeSpanTypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return new TimeSpan(Convert.ToInt64(outputValue));
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex))
            {
                return DBNull.Value;
            }
            return new TimeSpan(Convert.ToInt64(dataReader.GetValue(mapping.ColumnIndex)));
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return new TimeSpan(Convert.ToInt64(dataReader.GetValue(ordinal)));
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            TimeSpan span = (TimeSpan) parameterValue;
            dataParameter.Value = span.Ticks;
        }

        public override object ValueOf(Type type, string s)
        {
            return TimeSpan.Parse(s);
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

