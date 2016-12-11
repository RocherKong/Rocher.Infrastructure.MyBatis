namespace IBatisNet.DataMapper.Proxy
{
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LazyFactoryBuilder
    {
        private IDictionary _factory = new HybridDictionary();

        public LazyFactoryBuilder()
        {
            this._factory[typeof(IList)] = new LazyListFactory();
            this._factory[typeof(IList<>)] = new LazyListGenericFactory();
        }

        public ILazyFactory GetLazyFactory(Type type)
        {
            if (!type.IsInterface)
            {
                return new LazyLoadProxyFactory();
            }
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                return (this._factory[type.GetGenericTypeDefinition()] as ILazyFactory);
            }
            if (type != typeof(IList))
            {
                throw new DataMapperException("Cannot proxy others interfaces than IList or IList<>.");
            }
            return (this._factory[type] as ILazyFactory);
        }

        public void Register(Type type, string memberName, ILazyFactory factory)
        {
        }
    }
}

