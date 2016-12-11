namespace IBatisNet.DataMapper.DataExchange
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using System;

    public sealed class DotNetObjectDataExchange : BaseDataExchange
    {
        private Type _parameterClass;

        public DotNetObjectDataExchange(Type parameterClass, DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
        {
            this._parameterClass = parameterClass;
        }

        public override object GetData(ParameterProperty mapping, object parameterObject)
        {
            if (!mapping.IsComplexMemberName && (this._parameterClass == parameterObject.GetType()))
            {
                return mapping.GetAccessor.Get(parameterObject);
            }
            return ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName, base.DataExchangeFactory.AccessorFactory);
        }

        public override void SetData(ref object target, ParameterProperty mapping, object dataBaseValue)
        {
            if (mapping.IsComplexMemberName)
            {
                ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
            }
            else
            {
                base.DataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(this._parameterClass, mapping.PropertyName).Set(target, dataBaseValue);
            }
        }

        public override void SetData(ref object target, ResultProperty mapping, object dataBaseValue)
        {
            Type c = target.GetType();
            if (((c != this._parameterClass) && !c.IsSubclassOf(this._parameterClass)) && !this._parameterClass.IsAssignableFrom(c))
            {
                throw new ArgumentException(string.Concat(new object[] { "Could not set value in class '", target.GetType(), "' for property '", mapping.PropertyName, "' of type '", mapping.MemberType, "'" }));
            }
            if (mapping.IsComplexMemberName)
            {
                ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue, base.DataExchangeFactory.ObjectFactory, base.DataExchangeFactory.AccessorFactory);
            }
            else
            {
                mapping.SetAccessor.Set(target, dataBaseValue);
            }
        }
    }
}

