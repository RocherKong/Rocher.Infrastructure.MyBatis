namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;

    public sealed class PostBindind
    {
        private object _keys;
        private ExecuteMethod _method = ExecuteMethod.ExecuteQueryForIList;
        private IBatisNet.DataMapper.Configuration.ResultMapping.ResultProperty _property;
        private IMappedStatement _statement;
        private object _target;

        public object Keys
        {
            get
            {
                return this._keys;
            }
            set
            {
                this._keys = value;
            }
        }

        public ExecuteMethod Method
        {
            get
            {
                return this._method;
            }
            set
            {
                this._method = value;
            }
        }

        public IBatisNet.DataMapper.Configuration.ResultMapping.ResultProperty ResultProperty
        {
            get
            {
                return this._property;
            }
            set
            {
                this._property = value;
            }
        }

        public IMappedStatement Statement
        {
            get
            {
                return this._statement;
            }
            set
            {
                this._statement = value;
            }
        }

        public object Target
        {
            get
            {
                return this._target;
            }
            set
            {
                this._target = value;
            }
        }

        public enum ExecuteMethod
        {
            ExecuteQueryForArrayList = 4,
            ExecuteQueryForGenericIList = 3,
            ExecuteQueryForIList = 2,
            ExecuteQueryForObject = 1,
            ExecuteQueryForStrongTypedIList = 5
        }
    }
}

