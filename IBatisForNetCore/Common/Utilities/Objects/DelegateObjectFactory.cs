namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Logging;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text;

    public sealed class DelegateObjectFactory : IObjectFactory
    {
        private IDictionary _cachedfactories = new HybridDictionary();
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private object _padlock = new object();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IFactory CreateFactory(Type typeToCreate, Type[] types)
        {
            string str = this.GenerateKey(typeToCreate, types);
            IFactory factory = this._cachedfactories[str] as IFactory;
            if (factory == null)
            {
                lock (this._padlock)
                {
                    factory = this._cachedfactories[str] as IFactory;
                    if (factory != null)
                    {
                        return factory;
                    }
                    if (typeToCreate.IsAbstract)
                    {
                        if (_logger.IsInfoEnabled)
                        {
                            _logger.Info("Create a stub IFactory for abstract type " + typeToCreate.Name);
                        }
                        factory = new AbstractFactory(typeToCreate);
                    }
                    else
                    {
                        factory = new DelegateFactory(typeToCreate, types);
                    }
                    this._cachedfactories[str] = factory;
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

