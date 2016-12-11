namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;

    public interface IAccessor
    {
        Type MemberType { get; }

        string Name { get; }
    }
}

