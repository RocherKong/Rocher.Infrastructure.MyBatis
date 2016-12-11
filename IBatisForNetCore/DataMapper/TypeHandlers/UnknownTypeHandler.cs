namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public sealed class UnknownTypeHandler : BaseTypeHandler
    {
        private TypeHandlerFactory _factory;

        public UnknownTypeHandler(TypeHandlerFactory factory)
        {
            this._factory = factory;
        }

        public override bool Equals(object obj, string str)
        {
            if ((obj == null) || (str == null))
            {
                return (((string) obj) == str);
            }
            object obj2 = this._factory.GetTypeHandler(obj.GetType()).ValueOf(obj.GetType(), str);
            return obj.Equals(obj2);
        }

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
            return dataReader.GetValue(mapping.ColumnIndex);
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (dataReader.IsDBNull(ordinal))
            {
                return DBNull.Value;
            }
            return dataReader.GetValue(ordinal);
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            if (parameterValue != null)
            {
                this._factory.GetTypeHandler(parameterValue.GetType(), dbType).SetParameter(dataParameter, parameterValue, dbType);
            }
            else
            {
                dataParameter.Value = DBNull.Value;
            }
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

