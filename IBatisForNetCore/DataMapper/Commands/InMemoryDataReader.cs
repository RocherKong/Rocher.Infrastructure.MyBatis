namespace IBatisNet.DataMapper.Commands
{
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Data;
    using System.Reflection;

    public class InMemoryDataReader : IDataReader, IDisposable, IDataRecord
    {
        private int _currentResultIndex;
        private int _currentRowIndex;
        private bool _isClosed;
        private InMemoryResultSet[] _results;

        public InMemoryDataReader(IDataReader reader)
        {
            ArrayList list = new ArrayList();
            try
            {
                this._currentResultIndex = 0;
                this._currentRowIndex = 0;
                list.Add(new InMemoryResultSet(reader, true));
                while (reader.NextResult())
                {
                    list.Add(new InMemoryResultSet(reader, false));
                }
                this._results = (InMemoryResultSet[]) list.ToArray(typeof(InMemoryResultSet));
            }
            catch (Exception exception)
            {
                throw new DataMapperException("There was a problem converting an IDataReader to an InMemoryDataReader", exception);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
            }
        }

        public void Close()
        {
            this._isClosed = true;
        }

        public void Dispose()
        {
            this._isClosed = true;
            this._results = null;
        }

        public bool GetBoolean(int fieldIndex)
        {
            return (bool) this.GetValue(fieldIndex);
        }

        public byte GetByte(int fieldIndex)
        {
            return (byte) this.GetValue(fieldIndex);
        }

        public long GetBytes(int fieldIndex, long dataIndex, byte[] buffer, int bufferIndex, int length)
        {
            object obj2 = this.GetValue(fieldIndex);
            if (!(obj2 is byte[]))
            {
                throw new InvalidCastException("Type is " + obj2.GetType().ToString());
            }
            if (buffer == null)
            {
                return (long) ((byte[]) obj2).Length;
            }
            int num = ((byte[]) obj2).Length - ((int) dataIndex);
            if (num < length)
            {
                length = num;
            }
            Array.Copy((byte[]) obj2, (int) dataIndex, buffer, bufferIndex, length);
            return (long) length;
        }

        public char GetChar(int fieldIndex)
        {
            return (char) this.GetValue(fieldIndex);
        }

        public long GetChars(int fieldIndex, long dataIndex, char[] buffer, int bufferIndex, int length)
        {
            object obj2 = this.GetValue(fieldIndex);
            char[] sourceArray = null;
            if (obj2 is char[])
            {
                sourceArray = (char[]) obj2;
            }
            else
            {
                if (!(obj2 is string))
                {
                    throw new InvalidCastException("Type is " + obj2.GetType().ToString());
                }
                sourceArray = ((string) obj2).ToCharArray();
            }
            if (buffer == null)
            {
                return (long) sourceArray.Length;
            }
            Array.Copy(sourceArray, (int) dataIndex, buffer, bufferIndex, length);
            return (sourceArray.Length - dataIndex);
        }

        public IDataReader GetData(int fieldIndex)
        {
            throw new NotImplementedException("GetData(int) is not implemented, cause not use.");
        }

        public string GetDataTypeName(int fieldIndex)
        {
            return this.CurrentResultSet.GetDataTypeName(fieldIndex);
        }

        public DateTime GetDateTime(int fieldIndex)
        {
            return (DateTime) this.GetValue(fieldIndex);
        }

        public decimal GetDecimal(int fieldIndex)
        {
            return (decimal) this.GetValue(fieldIndex);
        }

        public double GetDouble(int fieldIndex)
        {
            return (double) this.GetValue(fieldIndex);
        }

        public Type GetFieldType(int fieldIndex)
        {
            return this.CurrentResultSet.GetFieldType(fieldIndex);
        }

        public float GetFloat(int fieldIndex)
        {
            return (float) this.GetValue(fieldIndex);
        }

        public Guid GetGuid(int fieldIndex)
        {
            return (Guid) this.GetValue(fieldIndex);
        }

        public short GetInt16(int fieldIndex)
        {
            return (short) this.GetValue(fieldIndex);
        }

        public int GetInt32(int fieldIndex)
        {
            return (int) this.GetValue(fieldIndex);
        }

        public long GetInt64(int fieldIndex)
        {
            return (long) this.GetValue(fieldIndex);
        }

        public string GetName(int fieldIndex)
        {
            return this.CurrentResultSet.GetName(fieldIndex);
        }

        public int GetOrdinal(string colName)
        {
            return this.CurrentResultSet.GetOrdinal(colName);
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException("GetSchemaTable() is not implemented, cause not use.");
        }

        public string GetString(int fieldIndex)
        {
            return (string) this.GetValue(fieldIndex);
        }

        public object GetValue(int fieldIndex)
        {
            return this.CurrentResultSet.GetValue(this._currentRowIndex, fieldIndex);
        }

        public int GetValues(object[] values)
        {
            return this.CurrentResultSet.GetValues(this._currentRowIndex, values);
        }

        public bool IsDBNull(int fieldIndex)
        {
            return (this.GetValue(fieldIndex) == DBNull.Value);
        }

        public bool NextResult()
        {
            this._currentResultIndex++;
            if (this._currentResultIndex >= this._results.Length)
            {
                this._currentResultIndex--;
                return false;
            }
            return true;
        }

        public bool Read()
        {
            this._currentRowIndex++;
            if (this._currentRowIndex >= this._results[this._currentResultIndex].RecordCount)
            {
                this._currentRowIndex--;
                return false;
            }
            return true;
        }

        private InMemoryResultSet CurrentResultSet
        {
            get
            {
                return this._results[this._currentResultIndex];
            }
        }

        public int Depth
        {
            get
            {
                return this._currentResultIndex;
            }
        }

        public int FieldCount
        {
            get
            {
                return this.CurrentResultSet.FieldCount;
            }
        }

        public bool IsClosed
        {
            get
            {
                return this._isClosed;
            }
        }

        public object this[string name]
        {
            get
            {
                return this[this.GetOrdinal(name)];
            }
        }

        public object this[int fieldIndex]
        {
            get
            {
                return this.GetValue(fieldIndex);
            }
        }

        public int RecordsAffected
        {
            get
            {
                throw new NotImplementedException("InMemoryDataReader only used for select IList statements !");
            }
        }

        private class InMemoryResultSet
        {
            private string[] _dataTypeName;
            private int _fieldCount;
            private string[] _fieldsName;
            private StringDictionary _fieldsNameLookup = new StringDictionary();
            private Type[] _fieldsType;
            private readonly object[][] _records;

            public InMemoryResultSet(IDataReader reader, bool isMidstream)
            {
                ArrayList list = new ArrayList();
                this._fieldCount = reader.FieldCount;
                this._fieldsName = new string[this._fieldCount];
                this._fieldsType = new Type[this._fieldCount];
                this._dataTypeName = new string[this._fieldCount];
                bool flag = true;
                while (isMidstream || reader.Read())
                {
                    if (flag)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string name = reader.GetName(i);
                            this._fieldsName[i] = name;
                            if (!this._fieldsNameLookup.ContainsKey(name))
                            {
                                this._fieldsNameLookup.Add(name, i.ToString());
                            }
                            this._fieldsType[i] = reader.GetFieldType(i);
                            this._dataTypeName[i] = reader.GetDataTypeName(i);
                        }
                    }
                    flag = false;
                    object[] values = new object[this._fieldCount];
                    reader.GetValues(values);
                    list.Add(values);
                    isMidstream = false;
                }
                this._records = (object[][]) list.ToArray(typeof(object[]));
            }

            public string GetDataTypeName(int colIndex)
            {
                return this._dataTypeName[colIndex];
            }

            public Type GetFieldType(int colIndex)
            {
                return this._fieldsType[colIndex];
            }

            public string GetName(int colIndex)
            {
                return this._fieldsName[colIndex];
            }

            public int GetOrdinal(string colName)
            {
                if (!this._fieldsNameLookup.ContainsKey(colName))
                {
                    throw new IndexOutOfRangeException(string.Format("No column with the specified name was found: {0}.", colName));
                }
                return Convert.ToInt32(this._fieldsNameLookup[colName]);
            }

            public object GetValue(int rowIndex, int colIndex)
            {
                return this._records[rowIndex][colIndex];
            }

            public int GetValues(int rowIndex, object[] values)
            {
                Array.Copy(this._records[rowIndex], 0, values, 0, this._fieldCount);
                return this._fieldCount;
            }

            public int FieldCount
            {
                get
                {
                    return this._fieldCount;
                }
            }

            public int RecordCount
            {
                get
                {
                    return this._records.Length;
                }
            }
        }
    }
}

