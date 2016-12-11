namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class EmitPropertyGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
    {
        private bool _canRead;
        private IGet _emittedGet;
        private string _propertyName = string.Empty;
        private Type _propertyType;
        private Type _targetType;

        public EmitPropertyGetAccessor(Type targetObjectType, string propertyName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
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
            this._canRead = property.CanRead;
            this.EmitIL(assemblyBuilder, moduleBuilder);
        }

        private void EmitIL(AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            this.EmitType(moduleBuilder);
            this._emittedGet = assemblyBuilder.CreateInstance("GetFor" + this._targetType.FullName + this._propertyName) as IGet;
            base.nullInternal = base.GetNullInternal(this._propertyType);
            if (this._emittedGet == null)
            {
                throw new NotSupportedException(string.Format("Unable to create a get property accessor for \"{0}\".", this._propertyType));
            }
        }

        private void EmitType(ModuleBuilder moduleBuilder)
        {
            TypeBuilder builder = moduleBuilder.DefineType("GetFor" + this._targetType.FullName + this._propertyName, TypeAttributes.Sealed | TypeAttributes.Public);
            builder.AddInterfaceImplementation(typeof(IGet));
            builder.DefineDefaultConstructor(MethodAttributes.Public);
            Type[] parameterTypes = new Type[] { typeof(object) };
            ILGenerator iLGenerator = builder.DefineMethod("Get", MethodAttributes.Virtual | MethodAttributes.Public, typeof(object), parameterTypes).GetILGenerator();
            if (this._canRead)
            {
                MethodInfo method = this._targetType.GetMethod("get_" + this._propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (method == null)
                {
                    method = this._targetType.GetMethod("get_" + this._propertyName);
                }
                iLGenerator.DeclareLocal(typeof(object));
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Castclass, this._targetType);
                iLGenerator.EmitCall(OpCodes.Call, method, null);
                if (method.ReturnType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, method.ReturnType);
                }
                iLGenerator.Emit(OpCodes.Stloc_0);
                iLGenerator.Emit(OpCodes.Ldloc_0);
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
            if (!this._canRead)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} doesn't have a get method.", this._propertyName, this._targetType));
            }
            return this._emittedGet.Get(target);
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

