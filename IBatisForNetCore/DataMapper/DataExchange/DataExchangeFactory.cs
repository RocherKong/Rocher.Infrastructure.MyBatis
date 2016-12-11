namespace IBatisNet.DataMapper.DataExchange
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;

    public class DataExchangeFactory
    {
        private IBatisNet.Common.Utilities.Objects.Members.AccessorFactory _accessorFactory;
        private IDataExchange _complexDataExchange;
        private IDataExchange _dictionaryDataExchange;
        private IDataExchange _listDataExchange;
        private IObjectFactory _objectFactory;
        private IDataExchange _primitiveDataExchange;
        private IBatisNet.DataMapper.TypeHandlers.TypeHandlerFactory _typeHandlerFactory;

        public DataExchangeFactory(IBatisNet.DataMapper.TypeHandlers.TypeHandlerFactory typeHandlerFactory, IObjectFactory objectFactory, IBatisNet.Common.Utilities.Objects.Members.AccessorFactory accessorFactory)
        {
            this._objectFactory = objectFactory;
            this._typeHandlerFactory = typeHandlerFactory;
            this._accessorFactory = accessorFactory;
            this._primitiveDataExchange = new PrimitiveDataExchange(this);
            this._complexDataExchange = new ComplexDataExchange(this);
            this._listDataExchange = new ListDataExchange(this);
            this._dictionaryDataExchange = new DictionaryDataExchange(this);
        }

        public IDataExchange GetDataExchangeForClass(Type clazz)
        {
            if (clazz == null)
            {
                return this._complexDataExchange;
            }
            if (typeof(IList).IsAssignableFrom(clazz))
            {
                return this._listDataExchange;
            }
            if (typeof(IDictionary).IsAssignableFrom(clazz))
            {
                return this._dictionaryDataExchange;
            }
            if (this._typeHandlerFactory.GetTypeHandler(clazz) != null)
            {
                return this._primitiveDataExchange;
            }
            return new DotNetObjectDataExchange(clazz, this);
        }

        public IBatisNet.Common.Utilities.Objects.Members.AccessorFactory AccessorFactory
        {
            get
            {
                return this._accessorFactory;
            }
        }

        public IObjectFactory ObjectFactory
        {
            get
            {
                return this._objectFactory;
            }
        }

        public IBatisNet.DataMapper.TypeHandlers.TypeHandlerFactory TypeHandlerFactory
        {
            get
            {
                return this._typeHandlerFactory;
            }
        }
    }
}

