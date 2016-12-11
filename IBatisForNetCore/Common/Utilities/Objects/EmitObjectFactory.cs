namespace IBatisNet.Common.Utilities.Objects
{
    using System;
    using System.Collections;
    using System.Text;

    public sealed class EmitObjectFactory : IObjectFactory
    {
        private IDictionary _cachedfactories = new HybridDictionary();
        private FactoryBuilder _factoryBuilder = new FactoryBuilder();
        private object _padlock = new object();

        public IFactory CreateFactory(Type typeToCreate, Type[] types)
        {
            string str = this.GenerateKey(typeToCreate, types);
            IFactory factory = this._cachedfactories[str] as IFactory;
            if (factory == null)
            {
                lock (this._padlock)
                {
                    factory = this._cachedfactories[str] as IFactory;
                    if (factory == null)
                    {
                        factory = this._factoryBuilder.CreateFactory(typeToCreate, types);
                        this._cachedfactories[str] = factory;
                    }
                }
            }
            return factory;
        }

        private string GenerateKey(Type typeToCreate, object[] arguments)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(typeToCreate.ToString());
            builder.Append(".");
            if ((arguments != null) && (arguments.Length != 0))
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    builder.Append(".").Append(arguments[i]);
                }
            }
            return builder.ToString();
        }
    }
}

