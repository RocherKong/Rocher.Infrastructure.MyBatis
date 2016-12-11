namespace IBatisNet.DataMapper.DataExchange
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;

    public sealed class ComplexDataExchange : BaseDataExchange
    {
        public ComplexDataExchange(DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
        {
        }

        public override object GetData(ParameterProperty mapping, object parameterObject)
        {
            if (parameterObject == null)
            {
                return null;
            }
            if (base.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(parameterObject.GetType()))
            {
                return parameterObject;
            }
            return ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName, base.DataExchangeFactory.AccessorFactory);
        }

        public override void SetData(ref object target, ParameterProperty mapping, object dataBaseValue)
        {
            ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
        }

        public override void SetData(ref object target, ResultProperty mapping, object dataBaseValue)
        {
            ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
        }
    }
}

