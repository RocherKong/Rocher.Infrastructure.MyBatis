namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class EmitFieldGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
    {
        private IGet _emittedGet;
        private string _fieldName = string.Empty;
        private Type _fieldType;
        private Type _targetType;
        private const BindingFlags VISIBILITY = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        public EmitFieldGetAccessor(Type targetObjectType, string fieldName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
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
            this._emittedGet = assemblyBuilder.CreateInstance("GetFor" + this._targetType.FullName + this._fieldName) as IGet;
            base.nullInternal = base.GetNullInternal(this._fieldType);
            if (this._emittedGet == null)
            {
                throw new NotSupportedException(string.Format("Unable to create a get field accessor for '{0}' field on class  '{0}'.", this._fieldName, this._fieldType));
            }
        }

        private void EmitType(ModuleBuilder moduleBuilder)
        {
            TypeBuilder builder = moduleBuilder.DefineType("GetFor" + this._targetType.FullName + this._fieldName, TypeAttributes.Sealed | TypeAttributes.Public);
            builder.AddInterfaceImplementation(typeof(IGet));
            builder.DefineDefaultConstructor(MethodAttributes.Public);
            Type[] parameterTypes = new Type[] { typeof(object) };
            ILGenerator iLGenerator = builder.DefineMethod("Get", MethodAttributes.Virtual | MethodAttributes.Public, typeof(object), parameterTypes).GetILGenerator();
            FieldInfo field = this._targetType.GetField(this._fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Ldfld, field);
                if (this._fieldType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, field.FieldType);
                }
                iLGenerator.Emit(OpCodes.Ret);
            }
            else
            {
                iLGenerator.ThrowException(typeof(MissingMethodException));
            }
            builder.CreateType();
        }

        public object Get(object target)
        {
            return this._emittedGet.Get(target);
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

