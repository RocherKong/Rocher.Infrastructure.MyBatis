namespace IBatisNet.Common.Utilities
{
    using IBatisNet.Common.Utilities.TypesResolver;
    using System;
    using System.Collections.Generic;

    public sealed class TypeUtils
    {
        private static readonly ITypeResolver _internalTypeResolver = new CachedTypeResolver(new TypeResolver());

        private TypeUtils()
        {
        }

        public static object InstantiateNullableType(Type type)
        {
            object obj2 = null;
            if (type == typeof(bool?))
            {
                return (bool?) false;
            }
            if (type == typeof(byte?))
            {
                return (byte?) 0;
            }
            if (type == typeof(char?))
            {
                return (char?) '\0';
            }
            if (type == typeof(DateTime?))
            {
                return new DateTime?(DateTime.MinValue);
            }
            if (type == typeof(decimal?))
            {
                return (decimal?) -79228162514264337593543950335M;
            }
            if (type == typeof(double?))
            {
                return (double?) double.MinValue;
            }
            if (type == typeof(short?))
            {
                return (short?) (-32768);
            }
            if (type == typeof(int?))
            {
                return (int?) (-2147483648);
            }
            if (type == typeof(long?))
            {
                return (long?) (-9223372036854775808L);
            }
            if (type == typeof(sbyte?))
            {
                return (sbyte?) (-128);
            }
            if (type == typeof(float?))
            {
                return (float?) float.MinValue;
            }
            if (type == typeof(ushort?))
            {
                return (ushort?) 0;
            }
            if (type == typeof(uint?))
            {
                return (uint?) 0;
            }
            if (type == typeof(ulong?))
            {
                obj2 = (ulong?) 0L;
            }
            return obj2;
        }

        public static object InstantiatePrimitiveType(TypeCode typeCode)
        {
            object obj2 = null;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return false;

                case TypeCode.Char:
                    return '\0';

                case TypeCode.SByte:
                    return (sbyte) 0;

                case TypeCode.Byte:
                    return (byte) 0;

                case TypeCode.Int16:
                    return (short) 0;

                case TypeCode.UInt16:
                    return (ushort) 0;

                case TypeCode.Int32:
                    return 0;

                case TypeCode.UInt32:
                    return 0;

                case TypeCode.Int64:
                    return 0L;

                case TypeCode.UInt64:
                    return (ulong) 0L;

                case TypeCode.Single:
                    return 0f;

                case TypeCode.Double:
                    return 0.0;

                case TypeCode.Decimal:
                    return 0M;

                case TypeCode.DateTime:
                    return new DateTime();

                case (TypeCode.DateTime | TypeCode.Object):
                    return obj2;

                case TypeCode.String:
                    return "";
            }
            return obj2;
        }

        public static bool IsImplementGenericIListInterface(Type type)
        {
            bool flag = false;
            if (!type.IsGenericType)
            {
                flag = false;
            }
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                return true;
            }
            foreach (Type type2 in type.GetInterfaces())
            {
                flag = IsImplementGenericIListInterface(type2);
                if (flag)
                {
                    return flag;
                }
            }
            return flag;
        }

        public static Type ResolveType(string typeName)
        {
            Type type = TypeRegistry.ResolveType(typeName);
            if (type == null)
            {
                type = _internalTypeResolver.Resolve(typeName);
            }
            return type;
        }
    }
}

