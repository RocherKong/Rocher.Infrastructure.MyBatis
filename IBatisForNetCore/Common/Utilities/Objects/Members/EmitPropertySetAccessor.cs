namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class EmitPropertySetAccessor : BaseAccessor, ISetAccessor, IAccessor, ISet
    {
        private bool _canWrite;
        private ISet _emittedSet;
        private string _propertyName = string.Empty;
        private Type _propertyType;
        private Type _targetType;

        public EmitPropertySetAccessor(Type targetObjectType, string propertyName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            this._targetType = targetObjectType;
            this._propertyName = propertyName;
            PropertyInfo property = this._targetType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (property == null)
            {
                property = this._targetType.GetProperty(propertyName);
            }
            if (property == null)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" does not exist for type {1}.", propertyName, this._targetType));
            }
            this._propertyType = property.PropertyType;
            this._canWrite = property.CanWrite;
            this.EmitIL(assemblyBuilder, moduleBuilder);
        }

        private void EmitIL(AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            this.EmitType(moduleBuilder);
            this._emittedSet = assemblyBuilder.CreateInstance("SetFor" + this._targetType.FullName + this._propertyName) as ISet;
            base.nullInternal = base.GetNullInternal(this._propertyType);
            if (this._emittedSet == null)
            {
                throw new NotSupportedException(string.Format("Unable to create a get propert accessor for '{0}' property on class  '{1}'.", this._propertyName, this._propertyType.ToString()));
            }
        }

        private void EmitType(ModuleBuilder moduleBuilder)
        {
            TypeBuilder builder = moduleBuilder.DefineType("SetFor" + this._targetType.FullName + this._propertyName, TypeAttributes.Sealed | TypeAttributes.Public);
            builder.AddInterfaceImplementation(typeof(ISet));
            builder.DefineDefaultConstructor(MethodAttributes.Public);
            Type[] parameterTypes = new Type[] { typeof(object), typeof(object) };
            ILGenerator iLGenerator = builder.DefineMethod("Set", MethodAttributes.Virtual | MethodAttributes.Public, null, parameterTypes).GetILGenerator();
            if (this._canWrite)
            {
                MethodInfo method = this._targetType.GetMethod("set_" + this._propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (method == null)
                {
                    method = this._targetType.GetMethod("set_" + this._propertyName);
                }
                Type parameterType = method.GetParameters()[0].ParameterType;
                iLGenerator.DeclareLocal(parameterType);
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Castclass, this._targetType);
                iLGenerator.Emit(OpCodes.Ldarg_2);
                if (parameterType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Unbox, parameterType);
                    if (BaseAccessor.typeToOpcode[parameterType] != null)
                    {
                        OpCode opcode = (OpCode) BaseAccessor.typeToOpcode[parameterType];
                        iLGenerator.Emit(opcode);
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldobj, parameterType);
                    }
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Castclass, parameterType);
                }
                iLGenerator.EmitCall(OpCodes.Callvirt, method, null);
                iLGenerator.Emit(OpCodes.Ret);
            }
            else
            {
                iLGenerator.ThrowException(typeof(MissingMethodException));
            }
            builder.CreateType();
        }

        public void Set(object target, object value)
        {
            if (!this._canWrite)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} doesn't have a set method.", this._propertyName, this._targetType));
            }
            object nullInternal = value;
            if (nullInternal == null)
            {
                nullInternal = base.nullInternal;
            }
            this._emittedSet.Set(target, nullInternal);
        }

        public Type MemberType
        {
            get
            {
                return this._propertyType;
            }
        }

        public string Name
        {
            get
            {
                return this._propertyName;
            }
        }
    }
}

