namespace IBatisNet.Common.Utilities.Objects.Members
{
    using IBatisNet.Common.Utilities.Objects;
    using System;
    using System.Reflection;

    public sealed class ReflectionFieldGetAccessor : IGetAccessor, IAccessor, IGet
    {
        private FieldInfo _fieldInfo;

        public ReflectionFieldGetAccessor(Type targetType, string fieldName)
        {
            ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
            this._fieldInfo = (FieldInfo) instance.GetGetter(fieldName);
        }

        public object Get(object target)
        {
            return this._fieldInfo.GetValue(target);
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

