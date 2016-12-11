namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;

    public interface IGetAccessorFactory
    {
        IGetAccessor CreateGetAccessor(Type targetType, string name);
    }
}

