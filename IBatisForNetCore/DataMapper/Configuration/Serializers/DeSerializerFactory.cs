namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;

    public class DeSerializerFactory
    {
        private IDictionary _serializerMap = new HybridDictionary();

        public DeSerializerFactory(ConfigurationScope configScope)
        {
            this._serializerMap.Add("dynamic", new DynamicDeSerializer(configScope));
            this._serializerMap.Add("isEqual", new IsEqualDeSerializer(configScope));
            this._serializerMap.Add("isNotEqual", new IsNotEqualDeSerializer(configScope));
            this._serializerMap.Add("isGreaterEqual", new IsGreaterEqualDeSerializer(configScope));
            this._serializerMap.Add("isGreaterThan", new IsGreaterThanDeSerializer(configScope));
            this._serializerMap.Add("isLessEqual", new IsLessEqualDeSerializer(configScope));
            this._serializerMap.Add("isLessThan", new IsLessThanDeSerializer(configScope));
            this._serializerMap.Add("isNotEmpty", new IsNotEmptyDeSerializer(configScope));
            this._serializerMap.Add("isEmpty", new IsEmptyDeSerializer(configScope));
            this._serializerMap.Add("isNotNull", new IsNotNullDeSerializer(configScope));
            this._serializerMap.Add("isNotParameterPresent", new IsNotParameterPresentDeSerializer(configScope));
            this._serializerMap.Add("isNotPropertyAvailable", new IsNotPropertyAvailableDeSerializer(configScope));
            this._serializerMap.Add("isNull", new IsNullDeSerializer(configScope));
            this._serializerMap.Add("isParameterPresent", new IsParameterPresentDeSerializer(configScope));
            this._serializerMap.Add("isPropertyAvailable", new IsPropertyAvailableDeSerializer(configScope));
            this._serializerMap.Add("iterate", new IterateSerializer(configScope));
        }

        public IDeSerializer GetDeSerializer(string name)
        {
            return (IDeSerializer) this._serializerMap[name];
        }
    }
}

