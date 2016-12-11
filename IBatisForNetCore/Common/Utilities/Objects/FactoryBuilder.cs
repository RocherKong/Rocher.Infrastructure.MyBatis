namespace IBatisNet.Common.Utilities.Objects
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public class FactoryBuilder
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ModuleBuilder _moduleBuilder;
        private const MethodAttributes CREATE_METHOD_ATTRIBUTES = (MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.Public);
        private const BindingFlags VISIBILITY = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public FactoryBuilder()
        {
            AssemblyName name = new AssemblyName {
                Name = "iBATIS.EmitFactory" + HashCodeProvider.GetIdentityHashCode(this).ToString()
            };
            this._moduleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run).DefineDynamicModule(name.Name + ".dll");
        }

        public IFactory CreateFactory(Type typeToCreate, Type[] types)
        {
            if (!typeToCreate.IsAbstract)
            {
                return (IFactory) this.CreateFactoryType(typeToCreate, types).GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            }
            if (_logger.IsInfoEnabled)
            {
                _logger.Info("Create a stub IFactory for abstract type " + typeToCreate.Name);
            }
            return new AbstractFactory(typeToCreate);
        }

        private Type CreateFactoryType(Type typeToCreate, Type[] types)
        {
            string str = string.Empty;
            for (int i = 0; i < types.Length; i++)
            {
                string introduced3 = types[i].Name.Replace("[]", string.Empty);
                str = str + introduced3 + i.ToString();
            }
            TypeBuilder typeBuilder = this._moduleBuilder.DefineType("EmitFactoryFor" + typeToCreate.FullName + str, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IFactory));
            this.ImplementCreateInstance(typeBuilder, typeToCreate, types);
            return typeBuilder.CreateType();
        }

        private void EmitArgsIL(ILGenerator il, Type[] argumentTypes)
        {
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
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

        private void ImplementCreateInstance(TypeBuilder typeBuilder, Type typeToCreate, Type[] argumentTypes)
        {
            ILGenerator iLGenerator = typeBuilder.DefineMethod("CreateInstance", MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.Public, typeof(object), new Type[] { typeof(object[]) }).GetILGenerator();
            ConstructorInfo con = typeToCreate.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, argumentTypes, null);
            if ((con == null) || !con.IsPublic)
            {
                throw new ProbeException(string.Format("Unable to optimize create instance. Cause : Could not find public constructor matching specified arguments for type \"{0}\".", typeToCreate.Name));
            }
            this.EmitArgsIL(iLGenerator, argumentTypes);
            iLGenerator.Emit(OpCodes.Newobj, con);
            iLGenerator.Emit(OpCodes.Ret);
        }
    }
}

