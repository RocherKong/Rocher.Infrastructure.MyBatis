namespace IBatisNet.Common.Utilities.Objects.Members
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public sealed class DelegateFieldSetAccessor : BaseAccessor, ISetAccessor, IAccessor, ISet
    {
        private string _fieldName = string.Empty;
        private Type _fieldType;
        private SetValue _set;

        public DelegateFieldSetAccessor(Type targetObjectType, string fieldName)
        {
            this._fieldName = fieldName;
            FieldInfo field = targetObjectType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                throw new NotSupportedException(string.Format("Field \"{0}\" does not exist for type {1}.", fieldName, targetObjectType));
            }
            this._fieldType = field.FieldType;
            base.nullInternal = base.GetNullInternal(this._fieldType);
            DynamicMethod method = new DynamicMethod("SetImplementation", null, new Type[] { typeof(object), typeof(object) }, base.GetType().Module, false);
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(field.FieldType, iLGenerator);
            iLGenerator.Emit(OpCodes.Stfld, field);
            iLGenerator.Emit(OpCodes.Ret);
            this._set = (SetValue) method.CreateDelegate(typeof(SetValue));
        }

        public void Set(object target, object value)
        {
            object nullInternal = value;
            if (nullInternal == null)
            {
                nullInternal = base.nullInternal;
            }
            this._set(target, nullInternal);
        }

        private static void UnboxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
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

        private delegate void SetValue(object instance, object value);
    }
}

