namespace IBatisNet.DataMapper.Configuration.ParameterMapping
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Data;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("parameter", Namespace="http://ibatis.apache.org/mapping")]
    public class ParameterProperty
    {
        [NonSerialized]
        private string _callBackName = string.Empty;
        [NonSerialized]
        private string _clrType = string.Empty;
        [NonSerialized]
        private string _columnName = string.Empty;
        [NonSerialized]
        private string _dbType;
        [NonSerialized]
        private ParameterDirection _direction = ParameterDirection.Input;
        [NonSerialized]
        private string _directionAttribute = string.Empty;
        [NonSerialized]
        private IGetAccessor _getAccessor;
        [NonSerialized]
        private bool _isComplexMemberName;
        [NonSerialized]
        private string _nullValue;
        [NonSerialized]
        private byte _precision;
        [NonSerialized]
        private string _propertyName = string.Empty;
        [NonSerialized]
        private byte _scale;
        [NonSerialized]
        private int _size = -1;
        [NonSerialized]
        private ITypeHandler _typeHandler;

        public ParameterProperty Clone()
        {
            return new ParameterProperty { CallBackName = this.CallBackName, CLRType = this.CLRType, ColumnName = this.ColumnName, DbType = this.DbType, DirectionAttribute = this.DirectionAttribute, NullValue = this.NullValue, PropertyName = this.PropertyName, Precision = this.Precision, Scale = this.Scale, Size = this.Size };
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (base.GetType() != obj.GetType()))
            {
                return false;
            }
            ParameterProperty property = (ParameterProperty) obj;
            return (this.PropertyName == property.PropertyName);
        }

        public override int GetHashCode()
        {
            return this._propertyName.GetHashCode();
        }

        public void Initialize(IScope scope, Type parameterClass)
        {
            if (this._directionAttribute.Length > 0)
            {
                this._direction = (ParameterDirection) Enum.Parse(typeof(ParameterDirection), this._directionAttribute, true);
            }
            if ((!typeof(IDictionary).IsAssignableFrom(parameterClass) && (parameterClass != null)) && !scope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(parameterClass))
            {
                if (!this._isComplexMemberName)
                {
                    this._getAccessor = scope.DataExchangeFactory.AccessorFactory.GetAccessorFactory.CreateGetAccessor(parameterClass, this._propertyName);
                }
                else
                {
                    string name = this._propertyName.Substring(this._propertyName.LastIndexOf('.') + 1);
                    string memberName = this._propertyName.Substring(0, this._propertyName.LastIndexOf('.'));
                    Type memberTypeForGetter = ObjectProbe.GetMemberTypeForGetter(parameterClass, memberName);
                    this._getAccessor = scope.DataExchangeFactory.AccessorFactory.GetAccessorFactory.CreateGetAccessor(memberTypeForGetter, name);
                }
            }
            scope.ErrorContext.MoreInfo = "Check the parameter mapping typeHandler attribute '" + this.CallBackName + "' (must be a ITypeHandlerCallback implementation).";
            if (this.CallBackName.Length > 0)
            {
                try
                {
                    ITypeHandlerCallback callback = (ITypeHandlerCallback) Activator.CreateInstance(scope.DataExchangeFactory.TypeHandlerFactory.GetType(this.CallBackName));
                    this._typeHandler = new CustomTypeHandler(callback);
                    return;
                }
                catch (Exception exception)
                {
                    throw new ConfigurationException("Error occurred during custom type handler configuration.  Cause: " + exception.Message, exception);
                }
            }
            if (this.CLRType.Length == 0)
            {
                if ((this._getAccessor != null) && scope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(this._getAccessor.MemberType))
                {
                    this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(this._getAccessor.MemberType, this._dbType);
                }
                else
                {
                    this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
            }
            else
            {
                Type type = TypeUtils.ResolveType(this.CLRType);
                if (scope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(type))
                {
                    this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, this._dbType);
                }
                else
                {
                    type = ObjectProbe.GetMemberTypeForGetter(type, this.PropertyName);
                    this._typeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, this._dbType);
                }
            }
        }

        [XmlAttribute("typeHandler")]
        public string CallBackName
        {
            get
            {
                return this._callBackName;
            }
            set
            {
                this._callBackName = value;
            }
        }

        [XmlAttribute("type")]
        public string CLRType
        {
            get
            {
                return this._clrType;
            }
            set
            {
                this._clrType = value;
            }
        }

        [XmlAttribute("column")]
        public string ColumnName
        {
            get
            {
                return this._columnName;
            }
            set
            {
                this._columnName = value;
            }
        }

        [XmlAttribute("dbType")]
        public string DbType
        {
            get
            {
                return this._dbType;
            }
            set
            {
                this._dbType = value;
            }
        }

        [XmlIgnore]
        public ParameterDirection Direction
        {
            get
            {
                return this._direction;
            }
            set
            {
                this._direction = value;
                this._directionAttribute = this._direction.ToString();
            }
        }

        [XmlAttribute("direction")]
        public string DirectionAttribute
        {
            get
            {
                return this._directionAttribute;
            }
            set
            {
                this._directionAttribute = value;
            }
        }

        [XmlIgnore]
        public IGetAccessor GetAccessor
        {
            get
            {
                return this._getAccessor;
            }
        }

        [XmlIgnore]
        public bool HasNullValue
        {
            get
            {
                return (this._nullValue != null);
            }
        }

        public bool IsComplexMemberName
        {
            get
            {
                return this._isComplexMemberName;
            }
        }

        [XmlAttribute("nullValue")]
        public string NullValue
        {
            get
            {
                return this._nullValue;
            }
            set
            {
                this._nullValue = value;
            }
        }

        [XmlAttribute("precision")]
        public byte Precision
        {
            get
            {
                return this._precision;
            }
            set
            {
                this._precision = value;
            }
        }

        [XmlAttribute("property")]
        public string PropertyName
        {
            get
            {
                return this._propertyName;
            }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The property attribute is mandatory in a paremeter property.");
                }
                this._propertyName = value;
                if (this._propertyName.IndexOf('.') < 0)
                {
                    this._isComplexMemberName = false;
                }
                else
                {
                    this._isComplexMemberName = true;
                }
            }
        }

        [XmlAttribute("scale")]
        public byte Scale
        {
            get
            {
                return this._scale;
            }
            set
            {
                this._scale = value;
            }
        }

        [XmlAttribute("size")]
        public int Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
            }
        }

        [XmlIgnore]
        public ITypeHandler TypeHandler
        {
            get
            {
                return this._typeHandler;
            }
            set
            {
                this._typeHandler = value;
            }
        }
    }
}

