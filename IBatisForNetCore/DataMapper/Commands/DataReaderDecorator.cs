namespace IBatisNet.DataMapper.Commands
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;

    public class DataReaderDecorator : IDataReader, IDisposable, IDataRecord
    {
        private IDataReader _innerDataReader;
        private RequestScope _request;

        public DataReaderDecorator(IDataReader dataReader, RequestScope request)
        {
            this._innerDataReader = dataReader;
            this._request = request;
        }

        void IDataReader.Close()
        {
            this._innerDataReader.Close();
        }

        DataTable IDataReader.GetSchemaTable()
        {
            return this._innerDataReader.GetSchemaTable();
        }

        bool IDataReader.NextResult()
        {
            this._request.MoveNextResultMap();
            return this._innerDataReader.NextResult();
        }

        bool IDataReader.Read()
        {
            return this._innerDataReader.Read();
        }

        bool IDataRecord.GetBoolean(int i)
        {
            return this._innerDataReader.GetBoolean(i);
        }

        byte IDataRecord.GetByte(int i)
        {
            return this._innerDataReader.GetByte(i);
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._innerDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        char IDataRecord.GetChar(int i)
        {
            return this._innerDataReader.GetChar(i);
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._innerDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            return this._innerDataReader.GetData(i);
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            return this._innerDataReader.GetDataTypeName(i);
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            return this._innerDataReader.GetDateTime(i);
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            return this._innerDataReader.GetDecimal(i);
        }

        double IDataRecord.GetDouble(int i)
        {
            return this._innerDataReader.GetDouble(i);
        }

        Type IDataRecord.GetFieldType(int i)
        {
            return this._innerDataReader.GetFieldType(i);
        }

        float IDataRecord.GetFloat(int i)
        {
            return this._innerDataReader.GetFloat(i);
        }

        Guid IDataRecord.GetGuid(int i)
        {
            return this._innerDataReader.GetGuid(i);
        }

        short IDataRecord.GetInt16(int i)
        {
            return this._innerDataReader.GetInt16(i);
        }

        int IDataRecord.GetInt32(int i)
        {
            return this._innerDataReader.GetInt32(i);
        }

        long IDataRecord.GetInt64(int i)
        {
            return this._innerDataReader.GetInt64(i);
        }

        string IDataRecord.GetName(int i)
        {
            return this._innerDataReader.GetName(i);
        }

        int IDataRecord.GetOrdinal(string name)
        {
            return this._innerDataReader.GetOrdinal(name);
        }

        string IDataRecord.GetString(int i)
        {
            return this._innerDataReader.GetString(i);
        }

        object IDataRecord.GetValue(int i)
        {
            return this._innerDataReader.GetValue(i);
        }

        int IDataRecord.GetValues(object[] values)
        {
            return this._innerDataReader.GetValues(values);
        }

        bool IDataRecord.IsDBNull(int i)
        {
            return this._innerDataReader.IsDBNull(i);
        }

        void IDisposable.Dispose()
        {
            this._innerDataReader.Dispose();
        }

        int IDataReader.Depth
        {
            get
            {
                return this._innerDataReader.Depth;
            }
        }

        bool IDataReader.IsClosed
        {
            get
            {
                return this._innerDataReader.IsClosed;
            }
        }

        int IDataReader.RecordsAffected
        {
            get
            {
                return this._innerDataReader.RecordsAffected;
            }
        }

        int IDataRecord.FieldCount
        {
            get
            {
                return this._innerDataReader.FieldCount;
            }
        }

        object IDataRecord.this[string name]
        {
            get
            {
                return this._innerDataReader[name];
            }
        }

        object IDataRecord.this[int i]
        {
            get
            {
                return this._innerDataReader[i];
            }
        }
    }
}

