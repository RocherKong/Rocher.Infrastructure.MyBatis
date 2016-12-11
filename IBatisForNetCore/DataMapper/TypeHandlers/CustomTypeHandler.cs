namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;
    using System.Data;

    public sealed class CustomTypeHandler : BaseTypeHandler
    {
        private ITypeHandlerCallback _callback;

        public CustomTypeHandler(ITypeHandlerCallback callback)
        {
            this._callback = callback;
        }

        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            IResultGetter getter = new ResultGetterImpl(outputValue);
            return this._callback.GetResult(getter);
        }

        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            IResultGetter getter = new ResultGetterImpl(dataReader, mapping.ColumnIndex);
            return this._callback.GetResult(getter);
        }

        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            IResultGetter getter = new ResultGetterImpl(dataReader, mapping.ColumnName);
            return this._callback.GetResult(getter);
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType)
        {
            IParameterSetter setter = new ParameterSetterImpl(dataParameter);
            this._callback.SetParameter(setter, parameterValue);
        }

        public override object ValueOf(Type type, string s)
        {
            return this._callback.ValueOf(s);
        }

        public ITypeHandlerCallback Callback
        {
            get
            {
                return this._callback;
            }
            set
            {
            }
        }

        public override bool IsSimpleType
        {
            get
            {
                return true;
            }
        }

        public override object NullValue
        {
            get
            {
                return this._callback.NullValue;
            }
        }
    }
}

