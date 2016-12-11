namespace IBatisNet.Common.Utilities.Objects.Members
{
    using IBatisNet.Common.Utilities.Objects;
    using System;
    using System.Reflection;

    public sealed class ReflectionPropertyGetAccessor : IGetAccessor, IAccessor, IGet
    {
        private PropertyInfo _propertyInfo;
        private string _propertyName = string.Empty;
        private Type _targetType;

        public ReflectionPropertyGetAccessor(Type targetType, string propertyName)
        {
            ReflectionInfo instance = ReflectionInfo.GetInstance(targetType);
            this._propertyInfo = (PropertyInfo) instance.GetGetter(propertyName);
            this._targetType = targetType;
            this._propertyName = propertyName;
        }

        public object Get(object target)
        {
            if (!this._propertyInfo.CanRead)
            {
                throw new NotSupportedException(string.Format("Property \"{0}\" on type {1} doesn't have a get method.", this._propertyName, this._targetType));
            }
            return this._propertyInfo.GetValue(target, null);
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

