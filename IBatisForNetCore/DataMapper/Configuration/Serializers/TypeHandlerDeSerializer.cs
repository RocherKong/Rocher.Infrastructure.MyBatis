namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Alias;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class TypeHandlerDeSerializer
    {
        public static void Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            TypeHandler handler = new TypeHandler();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            handler.CallBackName = NodeUtils.GetStringAttribute(attributes, "callback");
            handler.ClassName = NodeUtils.GetStringAttribute(attributes, "type");
            handler.DbType = NodeUtils.GetStringAttribute(attributes, "dbType");
            handler.Initialize();
            configScope.ErrorContext.MoreInfo = "Check the callback attribute '" + handler.CallBackName + "' (must be a classname).";
            ITypeHandler handler2 = null;
            object obj2 = Activator.CreateInstance(configScope.SqlMapper.TypeHandlerFactory.GetType(handler.CallBackName));
            if (obj2 is ITypeHandlerCallback)
            {
                handler2 = new CustomTypeHandler((ITypeHandlerCallback) obj2);
            }
            else
            {
                if (!(obj2 is ITypeHandler))
                {
                    throw new ConfigurationException("The callBack type is not a valid implementation of ITypeHandler or ITypeHandlerCallback");
                }
                handler2 = (ITypeHandler) obj2;
            }
            configScope.ErrorContext.MoreInfo = "Check the type attribute '" + handler.ClassName + "' (must be a class name) or the dbType '" + handler.DbType + "' (must be a DbType type name).";
            if ((handler.DbType != null) && (handler.DbType.Length > 0))
            {
                configScope.DataExchangeFactory.TypeHandlerFactory.Register(TypeUtils.ResolveType(handler.ClassName), handler.DbType, handler2);
            }
            else
            {
                configScope.DataExchangeFactory.TypeHandlerFactory.Register(TypeUtils.ResolveType(handler.ClassName), handler2);
            }
        }
    }
}

