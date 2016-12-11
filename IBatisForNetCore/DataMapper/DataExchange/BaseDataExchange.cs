namespace IBatisNet.DataMapper.DataExchange
{
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;

    public abstract class BaseDataExchange : IDataExchange
    {
        private IBatisNet.DataMapper.DataExchange.DataExchangeFactory _dataExchangeFactory;

        public BaseDataExchange(IBatisNet.DataMapper.DataExchange.DataExchangeFactory dataExchangeFactory)
        {
            this._dataExchangeFactory = dataExchangeFactory;
        }

        public abstract object GetData(ParameterProperty mapping, object parameterObject);
        public abstract void SetData(ref object target, ParameterProperty mapping, object dataBaseValue);
        public abstract void SetData(ref object target, ResultProperty mapping, object dataBaseValue);

        public IBatisNet.DataMapper.DataExchange.DataExchangeFactory DataExchangeFactory
        {
            get
            {
                return this._dataExchangeFactory;
            }
        }
    }
}

