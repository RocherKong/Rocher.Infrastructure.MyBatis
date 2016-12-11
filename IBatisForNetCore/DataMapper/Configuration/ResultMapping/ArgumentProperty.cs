namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using IBatisNet.Common.Utilities;
    using IBatisNet.DataMapper.MappedStatements.ArgumentStrategy;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Reflection;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("argument", Namespace="http://ibatis.apache.org/mapping")]
    public class ArgumentProperty : ResultProperty
    {
        [NonSerialized]
        private string _argumentName = string.Empty;
        [NonSerialized]
        private IArgumentStrategy _argumentStrategy;
        [NonSerialized]
        private Type _argumentType;

        public void Initialize(ConfigurationScope configScope, ConstructorInfo constructorInfo)
        {
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Name == this._argumentName)
                {
                    this._argumentType = parameters[i].ParameterType;
                    break;
                }
            }
            if ((base.CallBackName != null) && (base.CallBackName.Length > 0))
            {
                configScope.ErrorContext.MoreInfo = "Argument property (" + this._argumentName + "), check the typeHandler attribute '" + base.CallBackName + "' (must be a ITypeHandlerCallback implementation).";
                try
                {
                    ITypeHandlerCallback callback = (ITypeHandlerCallback) Activator.CreateInstance(configScope.SqlMapper.TypeHandlerFactory.GetType(base.CallBackName));
                    base.TypeHandler = new CustomTypeHandler(callback);
                    return;
                }
                catch (Exception exception)
                {
                    throw new ConfigurationErrorsException("Error occurred during custom type handler configuration.  Cause: " + exception.Message, exception);
                }
            }
            configScope.ErrorContext.MoreInfo = "Argument property (" + this._argumentName + ") set the typeHandler attribute.";
            base.TypeHandler = this.ResolveTypeHandler(configScope, this._argumentType, base.CLRType, base.DbType);
        }

        public ITypeHandler ResolveTypeHandler(ConfigurationScope configScope, Type argumenType, string clrType, string dbType)
        {
            ITypeHandler typeHandler = null;
            if (argumenType == null)
            {
                return configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
            }
            if (typeof(IDictionary).IsAssignableFrom(argumenType))
            {
                if ((clrType == null) || (clrType.Length == 0))
                {
                    return configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
                try
                {
                    Type type = TypeUtils.ResolveType(clrType);
                    return configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
                }
                catch (Exception exception)
                {
                    throw new ConfigurationErrorsException("Error. Could not set TypeHandler.  Cause: " + exception.Message, exception);
                }
            }
            if (configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(argumenType, dbType) != null)
            {
                return configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(argumenType, dbType);
            }
            if ((clrType == null) || (clrType.Length == 0))
            {
                return configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
            }
            try
            {
                Type type2 = TypeUtils.ResolveType(clrType);
                typeHandler = configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type2, dbType);
            }
            catch (Exception exception2)
            {
                throw new ConfigurationErrorsException("Error. Could not set TypeHandler.  Cause: " + exception2.Message, exception2);
            }
            return typeHandler;
        }

        [XmlAttribute("argumentName")]
        public string ArgumentName
        {
            get
            {
                return this._argumentName;
            }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The name attribute is mandatory in a argument tag.");
                }
                this._argumentName = value;
            }
        }

        [XmlIgnore]
        public override IArgumentStrategy ArgumentStrategy
        {
            get
            {
                return this._argumentStrategy;
            }
            set
            {
                this._argumentStrategy = value;
            }
        }

        [XmlAttribute("lazyLoad")]
        public override bool IsLazyLoad
        {
            get
            {
                return false;
            }
            set
            {
                throw new InvalidOperationException("Argument property cannot be lazy load.");
            }
        }

        [XmlIgnore]
        public override Type MemberType
        {
            get
            {
                return this._argumentType;
            }
        }
    }
}

