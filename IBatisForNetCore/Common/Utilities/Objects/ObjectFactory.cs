namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Logging;
    using System;

    public class ObjectFactory : IObjectFactory
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IObjectFactory _objectFactory;

        public ObjectFactory(bool allowCodeGeneration)
        {
            if (allowCodeGeneration)
            {
                if (Environment.Version.Major >= 2)
                {
                    this._objectFactory = new DelegateObjectFactory();
                }
                else
                {
                    this._objectFactory = new EmitObjectFactory();
                }
            }
            else
            {
                this._objectFactory = new ActivatorObjectFactory();
            }
        }

        public IFactory CreateFactory(Type typeToCreate, Type[] types)
        {
            if (_logger.IsDebugEnabled)
            {
                return new FactoryLogAdapter(typeToCreate, types, this._objectFactory.CreateFactory(typeToCreate, types));
            }
            return this._objectFactory.CreateFactory(typeToCreate, types);
        }
    }
}

