namespace IBatisNet.Common.Utilities.Objects
{
    using System;

    public class ActivatorObjectFactory : IObjectFactory
    {
        public IFactory CreateFactory(Type typeToCreate, Type[] types)
        {
            return new ActivatorFactory(typeToCreate);
        }
    }
}

