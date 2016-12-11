namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public sealed class DelegatePropertySetAccessor : BaseAccessor, ISetAccessor, IAccessor, ISet
    {
        private bool _canWrite;
        private Type _propertyType;
        private SetValue _set;

        public DelegatePropertySetAccessor(Type targetObjectType, string propName)
        {
            base.targetType = targetObjectType;
            base.propertyName = propName;
            PropertyInfo propertyInfo = base.GetPropertyInfo(targetObjectType);
            if (propertyInfo == null)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" does not exist for type {1}.", base.propertyName, base.targetType));
            }
            this._propertyType = propertyInfo.PropertyType;
            this._canWrite = propertyInfo.CanWrite;
            base.nullInternal = base.GetNullInternal(this._propertyType);
            if (propertyInfo.CanWrite)
            {
                DynamicMethod method = new DynamicMethod("SetImplementation", null, new Type[] { typeof(object), typeof(object) }, base.GetType().Module, true);
                ILGenerator iLGenerator = method.GetILGenerator();
                MethodInfo setMethod = propertyInfo.GetSetMethod();
                Type parameterType = setMethod.GetParameters()[0].ParameterType;
                iLGenerator.DeclareLocal(parameterType);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Castclass, base.targetType);
                iLGenerator.Emit(OpCodes.Ldarg_1);
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
                iLGenerator.EmitCall(OpCodes.Callvirt, setMethod, null);
                iLGenerator.Emit(OpCodes.Ret);
                this._set = (SetValue) method.CreateDelegate(typeof(SetValue));
            }
        }

        public void Set(object target, object value)
        {
            if (!this._canWrite)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} doesn't have a set method.", base.propertyName, base.targetType));
            }
            object nullInternal = value;
            if (nullInternal == null)
            {
                nullInternal = base.nullInternal;
            }
            this._set(target, nullInternal);
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
                return base.propertyName;
            }
        }

        private delegate void SetValue(object instance, object value);
    }
}

