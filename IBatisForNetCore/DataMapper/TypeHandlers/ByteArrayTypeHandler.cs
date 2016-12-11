namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Data;
    using System.Text;

    public sealed class ByteArrayTypeHandler : BaseTypeHandler
    {
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            throw new DataMapperException("NotSupportedException");
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (!dataReader.IsDBNull(mapping.ColumnIndex) && (dataReader.GetBytes(mapping.ColumnIndex, 0L, null, 0, 0) != 0L))
            {
                return this.GetValueByIndex(mapping.ColumnIndex, dataReader);
            }
            return DBNull.Value;
        }

        private byte[] GetValueByIndex(int columnIndex, IDataReader dataReader)
        {
            int length = (int) dataReader.GetBytes(columnIndex, 0L, null, 0, 0);
            byte[] buffer = new byte[length];
            dataReader.GetBytes(columnIndex, 0L, buffer, 0, length);
            return buffer;
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(mapping.ColumnName);
            if (!dataReader.IsDBNull(ordinal) && (dataReader.GetBytes(ordinal, 0L, null, 0, 0) != 0L))
            {
                return this.GetValueByIndex(ordinal, dataReader);
            }
            return DBNull.Value;
        }

        public override object ValueOf(Type type, string s)
        {
            return Encoding.Default.GetBytes(s);
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

