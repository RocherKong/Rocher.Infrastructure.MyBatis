namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;

    public interface ISetAccessorFactory
    {
        ISetAccessor CreateSetAccessor(Type targetType, string name);
    }
}

