namespace IBatisNet.DataMapper.TypeHandlers
{
    using System;
    using System.Data;

    public sealed class ResultGetterImpl : IResultGetter
    {
        private int _columnIndex;
        private string _columnName;
        private IDataReader _dataReader;
        private object _outputValue;

        public ResultGetterImpl(object outputValue)
        {
            this._columnIndex = -2147483648;
            this._columnName = string.Empty;
            this._outputValue = outputValue;
        }

        public ResultGetterImpl(IDataReader dataReader, int columnIndex)
        {
            this._columnIndex = -2147483648;
            this._columnName = string.Empty;
            this._columnIndex = columnIndex;
            this._dataReader = dataReader;
        }

        public ResultGetterImpl(IDataReader dataReader, string columnName)
        {
            this._columnIndex = -2147483648;
            this._columnName = string.Empty;
            this._columnName = columnName;
            this._dataReader = dataReader;
        }

        public IDataReader DataReader
        {
            get
            {
                return this._dataReader;
            }
        }

        public object Value
        {
            get
            {
                if (this._columnName.Length > 0)
                {
                    int ordinal = this._dataReader.GetOrdinal(this._columnName);
                    return this._dataReader.GetValue(ordinal);
                }
                if (this._columnIndex >= 0)
                {
                    return this._dataReader.GetValue(this._columnIndex);
                }
                return this._outputValue;
            }
        }
    }
}

