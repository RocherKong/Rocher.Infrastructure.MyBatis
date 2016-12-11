namespace IBatisNet.Common.Utilities.Objects
{
    using System;

    public interface IObjectFactory
    {
        IFactory CreateFactory(Type typeToCreate, Type[] types);
    }
}

