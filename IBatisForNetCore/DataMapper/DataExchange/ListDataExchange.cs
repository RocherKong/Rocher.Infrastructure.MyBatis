namespace IBatisNet.DataMapper.DataExchange
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;

    public sealed class ListDataExchange : BaseDataExchange
    {
        public ListDataExchange(DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
        {
        }

        public override object GetData(ParameterProperty mapping, object parameterObject)
        {
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

