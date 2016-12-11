namespace IBatisNet.DataMapper.TypeHandlers
{
    using System;
    using System.Data;

    public sealed class ParameterSetterImpl : IParameterSetter
    {
        private IDataParameter _dataParameter;

        public ParameterSetterImpl(IDataParameter dataParameter)
        {
            this._dataParameter = dataParameter;
        }

        public IDataParameter DataParameter
        {
            get
            {
                return this._dataParameter;
            }
        }

        public object Value
        {
            set
            {
                this._dataParameter.Value = value;
            }
        }
    }
}

