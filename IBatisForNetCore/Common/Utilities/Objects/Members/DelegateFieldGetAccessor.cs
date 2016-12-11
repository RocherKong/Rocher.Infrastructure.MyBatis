namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public sealed class DelegateFieldGetAccessor : BaseAccessor, IGetAccessor, IAccessor, IGet
    {
        private string _fieldName = string.Empty;
        private Type _fieldType;
        private GetValue _get;

        public DelegateFieldGetAccessor(Type targetObjectType, string fieldName)
        {
            this._fieldName = fieldName;
            FieldInfo field = targetObjectType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                throw new NotSupportedException(string.Format("Field \"{0}\" does not exist for type {1}.", fieldName, targetObjectType));
            }
            this._fieldType = field.FieldType;
            base.nullInternal = base.GetNullInternal(this._fieldType);
            DynamicMethod method = new DynamicMethod("GetImplementation", typeof(object), new Type[] { typeof(object) }, base.GetType().Module, false);
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, field);
            if (this._fieldType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box, field.FieldType);
            }
            iLGenerator.Emit(OpCodes.Ret);
            this._get = (GetValue) method.CreateDelegate(typeof(GetValue));
        }

        public object Get(object target)
        {
            return this._get(target);
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

        private delegate object GetValue(object instance);
    }
}

