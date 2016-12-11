namespace IBatisNet.Common.Utilities
{
    using System;
    using System.Reflection;

    public sealed class HashCodeProvider
    {
        private static MethodInfo getHashCodeMethodInfo = typeof(object).GetMethod("GetHashCode");

        public static int GetIdentityHashCode(object obj)
        {
            return (int) getHashCodeMethodInfo.Invoke(obj, null);
        }
    }
}

