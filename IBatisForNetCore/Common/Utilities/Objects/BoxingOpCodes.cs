namespace IBatisNet.Common.Utilities.Objects
{
    using System;
    using System.Collections;
    using System.Reflection.Emit;

    public sealed class BoxingOpCodes
    {
        private static IDictionary boxingOpCodes = new Hashtable();

        static BoxingOpCodes()
        {
            boxingOpCodes[typeof(sbyte)] = OpCodes.Ldind_I1;
            boxingOpCodes[typeof(short)] = OpCodes.Ldind_I2;
            boxingOpCodes[typeof(int)] = OpCodes.Ldind_I4;
            boxingOpCodes[typeof(long)] = OpCodes.Ldind_I8;
            boxingOpCodes[typeof(byte)] = OpCodes.Ldind_U1;
            boxingOpCodes[typeof(ushort)] = OpCodes.Ldind_U2;
            boxingOpCodes[typeof(uint)] = OpCodes.Ldind_U4;
            boxingOpCodes[typeof(ulong)] = OpCodes.Ldind_I8;
            boxingOpCodes[typeof(float)] = OpCodes.Ldind_R4;
            boxingOpCodes[typeof(double)] = OpCodes.Ldind_R8;
            boxingOpCodes[typeof(char)] = OpCodes.Ldind_U2;
            boxingOpCodes[typeof(bool)] = OpCodes.Ldind_I1;
        }

        public static OpCode GetOpCode(Type type)
        {
            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }
            return (OpCode) boxingOpCodes[type];
        }
    }
}

