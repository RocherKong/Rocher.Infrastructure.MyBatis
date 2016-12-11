namespace IBatisNet.Common.Utilities.Objects.Members
{
    using IBatisNet.Common.Utilities.Objects;
    using System;
    using System.Reflection;

    public sealed class ReflectionFieldSetAccessor : ISetAccessor, IAccessor, ISet
    {
        private FieldInfo _fieldInfo;

        public ReflectionFieldSetAccessor(Type targetType, string fieldName)
        {
            ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
            this._fieldInfo = (FieldInfo) instance.GetGetter(fieldName);
        }

        public void Set(object target, object value)
        {
            this._fieldInfo.SetValue(target, value);
        }

        public Type MemberType
        {
            get
            {
                return this._fieldInfo.FieldType;
            }
        }

        public string Name
        {
            get
            {
                return this._fieldInfo.Name;
            }
        }
    }
}

