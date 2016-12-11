namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;

    public class AccessorFactory
    {
        private IGetAccessorFactory _getAccessorFactory;
        private ISetAccessorFactory _setAccessorFactory;

        public AccessorFactory(ISetAccessorFactory setAccessorFactory, IGetAccessorFactory getAccessorFactory)
        {
            this._setAccessorFactory = setAccessorFactory;
            this._getAccessorFactory = getAccessorFactory;
        }

        public IGetAccessorFactory GetAccessorFactory
        {
            get
            {
                return this._getAccessorFactory;
            }
        }

        public ISetAccessorFactory SetAccessorFactory
        {
            get
            {
                return this._setAccessorFactory;
            }
        }
    }
}

