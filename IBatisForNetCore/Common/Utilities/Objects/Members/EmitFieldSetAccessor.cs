namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class EmitFieldSetAccessor : BaseAccessor, ISetAccessor, IAccessor, ISet
    {
        private ISet _emittedSet;
        private string _fieldName = string.Empty;
        private Type _fieldType;
        private Type _targetType;
        private const BindingFlags VISIBILITY = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public EmitFieldSetAccessor(Type targetObjectType, string fieldName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            this._targetType = targetObjectType;
            this._fieldName = fieldName;
            FieldInfo field = this._targetType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                throw new NotSupportedException(string.Format("Field \"{0}\" does not exist for type {1}.", fieldName, targetObjectType));
            }
            this._fieldType = field.FieldType;
            this.EmitIL(assemblyBuilder, moduleBuilder);
        }

        private void EmitIL(AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            this.EmitType(moduleBuilder);
            this._emittedSet = assemblyBuilder.CreateInstance("SetFor" + this._targetType.FullName + this._fieldName) as ISet;
            base.nullInternal = base.GetNullInternal(this._fieldType);
            if (this._emittedSet == null)
            {
                throw new NotSupportedException(string.Format("Unable to create a set field accessor for '{0}' field on class  '{0}'.", this._fieldName, this._fieldType));
            }
        }

        private void EmitType(ModuleBuilder moduleBuilder)
        {
            TypeBuilder builder = moduleBuilder.DefineType("SetFor" + this._targetType.FullName + this._fieldName, TypeAttributes.Sealed | TypeAttributes.Public);
            builder.AddInterfaceImplementation(typeof(ISet));
            builder.DefineDefaultConstructor(MethodAttributes.Public);
            Type[] parameterTypes = new Type[] { typeof(object), typeof(object) };
            ILGenerator iLGenerator = builder.DefineMethod("Set", MethodAttributes.Virtual | MethodAttributes.Public, null, parameterTypes).GetILGenerator();
            FieldInfo field = this._targetType.GetField(this._fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Castclass, this._targetType);
                iLGenerator.Emit(OpCodes.Ldarg_2);
                if (this._fieldType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Unbox, this._fieldType);
                    if (BaseAccessor.typeToOpcode[this._fieldType] != null)
                    {
                        OpCode opcode = (OpCode) BaseAccessor.typeToOpcode[this._fieldType];
                        iLGenerator.Emit(opcode);
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldobj, this._fieldType);
                    }
                    iLGenerator.Emit(OpCodes.Stfld, field);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Stfld, field);
                }
            }
            else
            {
                iLGenerator.ThrowException(typeof(MissingMethodException));
            }
            iLGenerator.Emit(OpCodes.Ret);
            builder.CreateType();
        }

        public void Set(object target, object value)
        {
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
                return this._fieldType;
            }
        }

        public string Name
        {
            get
            {
                return this._fieldName;
            }
        }
    }
}

