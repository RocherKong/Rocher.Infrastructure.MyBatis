namespace IBatisNet.Common.Utilities.Objects
{
    using System;

    public sealed class ActivatorFactory : IFactory
    {
        private Type _typeToCreate;

        public ActivatorFactory(Type typeToCreate)
        {
            this._typeToCreate = typeToCreate;
        }

        public object CreateInstance(object[] parameters)
        {
            return Activator.CreateInstance(this._typeToCreate, parameters);
        }
    }
}

