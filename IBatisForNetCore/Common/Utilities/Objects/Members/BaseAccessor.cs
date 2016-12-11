namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Reflection.Emit;

    public abstract class BaseAccessor
    {
        protected object nullInternal;
        protected string propertyName = string.Empty;
        protected Type targetType;
        protected static IDictionary typeToOpcode = new HybridDictionary();

        static BaseAccessor()
        {
            typeToOpcode[typeof(sbyte)] = OpCodes.Ldind_I1;
            typeToOpcode[typeof(byte)] = OpCodes.Ldind_U1;
            typeToOpcode[typeof(char)] = OpCodes.Ldind_U2;
            typeToOpcode[typeof(short)] = OpCodes.Ldind_I2;
            typeToOpcode[typeof(ushort)] = OpCodes.Ldind_U2;
            typeToOpcode[typeof(int)] = OpCodes.Ldind_I4;
            typeToOpcode[typeof(uint)] = OpCodes.Ldind_U4;
            typeToOpcode[typeof(long)] = OpCodes.Ldind_I8;
            typeToOpcode[typeof(ulong)] = OpCodes.Ldind_I8;
            typeToOpcode[typeof(bool)] = OpCodes.Ldind_I1;
            typeToOpcode[typeof(double)] = OpCodes.Ldind_R8;
            typeToOpcode[typeof(float)] = OpCodes.Ldind_R4;
        }

        protected BaseAccessor()
        {
        }

        protected object GetNullInternal(Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    return this.GetNullInternal(Enum.GetUnderlyingType(type));
                }
                if (type.IsPrimitive)
                {
                    if (type == typeof(int))
                    {
                        return 0;
                    }
                    if (type == typeof(double))
                    {
                        return 0.0;
                    }
                    if (type == typeof(short))
                    {
                        return (short) 0;
                    }
                    if (type == typeof(sbyte))
                    {
                        return (sbyte) 0;
                    }
                    if (type == typeof(long))
                    {
                        return 0L;
                    }
                    if (type == typeof(byte))
                    {
                        return (byte) 0;
                    }
                    if (type == typeof(ushort))
                    {
                        return (ushort) 0;
                    }
                    if (type == typeof(uint))
                    {
                        return 0;
                    }
                    if (type == typeof(ulong))
                    {
                        return (ulong) 0L;
                    }
                    if (type == typeof(ulong))
                    {
                        return (ulong) 0L;
                    }
                    if (type == typeof(float))
                    {
                        return 0f;
                    }
                    if (type == typeof(bool))
                    {
                        return false;
                    }
                    if (type == typeof(char))
                    {
                        return '\0';
                    }
                }
                else
                {
                    if (type == typeof(DateTime))
                    {
                        return DateTime.MinValue;
                    }
                    if (type == typeof(decimal))
                    {
                        return 0M;
                    }
                    if (type == typeof(Guid))
                    {
                        return Guid.Empty;
                    }
                    if (type == typeof(TimeSpan))
                    {
                        return new TimeSpan(0, 0, 0);
                    }
                }
            }
            return null;
        }

        protected PropertyInfo GetPropertyInfo(Type target)
        {
            PropertyInfo property = null;
            property = target.GetProperty(this.propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (property != null)
            {
                return property;
            }
            if (target.IsInterface)
            {
                foreach (Type type in target.GetInterfaces())
                {
                    property = this.GetPropertyInfo(type);
                    if (property != null)
                    {
                        return property;
                    }
                }
                return property;
            }
            return target.GetProperty(this.propertyName);
        }
    }
}

