namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Exceptions;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public sealed class DelegateFactory : IFactory
    {
        private Create _create;
        private const BindingFlags VISIBILITY = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public DelegateFactory(Type typeToCreate, Type[] argumentTypes)
        {
            DynamicMethod method = new DynamicMethod("CreateImplementation", typeof(object), new Type[] { typeof(object[]) }, base.GetType().Module, false);
            ILGenerator iLGenerator = method.GetILGenerator();
            ConstructorInfo con = typeToCreate.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, argumentTypes, null);
            if ((con == null) || !con.IsPublic)
            {
                throw new ProbeException(string.Format("Unable to optimize create instance. Cause : Could not find public constructor matching specified arguments for type \"{0}\".", typeToCreate.Name));
            }
            this.EmitArgsIL(iLGenerator, argumentTypes);
            iLGenerator.Emit(OpCodes.Newobj, con);
            iLGenerator.Emit(OpCodes.Ret);
            this._create = (Create) method.CreateDelegate(typeof(Create));
        }

        public object CreateInstance(object[] parameters)
        {
            return this._create(parameters);
        }

        private void EmitArgsIL(ILGenerator il, Type[] argumentTypes)
        {
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldelem_Ref);
                Type cls = argumentTypes[i];
                if (cls.IsValueType)
                {
                    if (cls.IsPrimitive || cls.IsEnum)
                    {
                        il.Emit(OpCodes.Unbox, cls);
                        il.Emit(BoxingOpCodes.GetOpCode(cls));
                    }
                    else if (cls.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox, cls);
                        il.Emit(OpCodes.Ldobj, cls);
                    }
                }
            }
        }

        private delegate object Create(object[] parameters);
    }
}

