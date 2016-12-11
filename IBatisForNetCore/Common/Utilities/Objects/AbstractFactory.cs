namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Exceptions;
    using System;

    public class AbstractFactory : IFactory
    {
        private Type _typeToCreate;

        public AbstractFactory(Type typeToCreate)
        {
            this._typeToCreate = typeToCreate;
        }

        public object CreateInstance(object[] parameters)
        {
            throw new ProbeException(string.Format("Unable to optimize create instance. Cause : Could not find public constructor on the abstract type \"{0}\".", this._typeToCreate.Name));
        }
    }
}

