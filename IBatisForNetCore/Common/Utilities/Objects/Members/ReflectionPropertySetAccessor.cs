namespace IBatisNet.Common.Utilities.Objects.Members
{
    using IBatisNet.Common.Utilities.Objects;
    using System;
    using System.Reflection;

    public sealed class ReflectionPropertySetAccessor : ISetAccessor, IAccessor, ISet
    {
        private PropertyInfo _propertyInfo;
        private string _propertyName = string.Empty;
        private Type _targetType;

        public ReflectionPropertySetAccessor(Type targetType, string propertyName)
        {
            ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
            this._propertyInfo = (PropertyInfo) instance.GetSetter(propertyName);
            this._targetType = targetType;
            this._propertyName = propertyName;
        }

        public void Set(object target, object value)
        {
            if (!this._propertyInfo.CanWrite)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} doesn't have a set method.", this._propertyName, this._targetType));
            }
            this._propertyInfo.SetValue(target, value, null);
        }

        public Type MemberType
        {
            get
            {
                return this._propertyInfo.PropertyType;
            }
        }

        public string Name
        {
            get
            {
                return this._propertyInfo.Name;
            }
        }
    }
}

