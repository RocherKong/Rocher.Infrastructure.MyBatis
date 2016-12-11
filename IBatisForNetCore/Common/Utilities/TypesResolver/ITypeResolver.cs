namespace IBatisNet.Common.Utilities.TypesResolver
{
    using System;

    public interface ITypeResolver
    {
        Type Resolve(string typeName);
    }
}

