namespace IBatisNet.Common.Utilities.Objects
{
    using System;

    public interface IFactory
    {
        object CreateInstance(object[] parameters);
    }
}

