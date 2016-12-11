namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public sealed class DelegatePropertyGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
    {
        private bool _canRead;
        private GetValue _get;
        private Type _propertyType;

        public DelegatePropertyGetAccessor(Type targetObjectType, string propertyName)
        {
            base.targetType = targetObjectType;
            base.propertyName = propertyName;
            PropertyInfo propertyInfo = base.GetPropertyInfo(targetObjectType);
            if (propertyInfo == null)
            {
                propertyInfo = base.targetType.GetProperty(propertyName);
            }
            if (propertyInfo == null)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" does not exist for type {1}.", propertyName, base.targetType));
            }
            this._propertyType = propertyInfo.PropertyType;
            this._canRead = propertyInfo.CanRead;
            base.nullInternal = base.GetNullInternal(this._propertyType);
            if (propertyInfo.CanRead)
            {
                DynamicMethod method = new DynamicMethod("GetImplementation", typeof(object), new Type[] { typeof(object) }, base.GetType().Module, true);
                ILGenerator iLGenerator = method.GetILGenerator();
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                iLGenerator.DeclareLocal(typeof(object));
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Castclass, targetObjectType);
                iLGenerator.EmitCall(OpCodes.Callvirt, getMethod, null);
                if (getMethod.ReturnType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, getMethod.ReturnType);
                }
                iLGenerator.Emit(OpCodes.Stloc_0);
                iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ret);
                this._get = (GetValue) method.CreateDelegate(typeof(GetValue));
            }
        }

        public object Get(object target)
        {
            if (!this._canRead)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} doesn't have a get method.", base.propertyName, base.targetType));
            }
            return this._get(target);
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

        private delegate object GetValue(object instance);
    }
}

