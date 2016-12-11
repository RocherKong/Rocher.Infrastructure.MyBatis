namespace IBatisNet.DataMapper.TypeHandlers
{
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities;
    using IBatisNet.DataMapper.Configuration.Alias;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.TypeHandlers.Nullables;
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    public class TypeHandlerFactory
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IDictionary _typeAliasMaps = new HybridDictionary();
        private IDictionary _typeHandlerMap = new HybridDictionary();
        private ITypeHandler _unknownTypeHandler;
        private const string NULL = "_NULL_TYPE_";

        public TypeHandlerFactory()
        {
            ITypeHandler handler = null;
            handler = new DBNullTypeHandler();
            this.Register(typeof(DBNull), handler);
            handler = new BooleanTypeHandler();
            this.Register(typeof(bool), handler);
            handler = new ByteTypeHandler();
            this.Register(typeof(byte), handler);
            handler = new CharTypeHandler();
            this.Register(typeof(char), handler);
            handler = new DateTimeTypeHandler();
            this.Register(typeof(DateTime), handler);
            handler = new DecimalTypeHandler();
            this.Register(typeof(decimal), handler);
            handler = new DoubleTypeHandler();
            this.Register(typeof(double), handler);
            handler = new Int16TypeHandler();
            this.Register(typeof(short), handler);
            handler = new Int32TypeHandler();
            this.Register(typeof(int), handler);
            handler = new Int64TypeHandler();
            this.Register(typeof(long), handler);
            handler = new SingleTypeHandler();
            this.Register(typeof(float), handler);
            handler = new StringTypeHandler();
            this.Register(typeof(string), handler);
            handler = new GuidTypeHandler();
            this.Register(typeof(Guid), handler);
            handler = new TimeSpanTypeHandler();
            this.Register(typeof(TimeSpan), handler);
            handler = new ByteArrayTypeHandler();
            this.Register(typeof(byte[]), handler);
            handler = new ObjectTypeHandler();
            this.Register(typeof(object), handler);
            handler = new EnumTypeHandler();
            this.Register(typeof(Enum), handler);
            handler = new UInt16TypeHandler();
            this.Register(typeof(ushort), handler);
            handler = new UInt32TypeHandler();
            this.Register(typeof(uint), handler);
            handler = new UInt64TypeHandler();
            this.Register(typeof(ulong), handler);
            handler = new SByteTypeHandler();
            this.Register(typeof(sbyte), handler);
            handler = new NullableBooleanTypeHandler();
            this.Register(typeof(bool?), handler);
            handler = new NullableByteTypeHandler();
            this.Register(typeof(byte?), handler);
            handler = new NullableCharTypeHandler();
            this.Register(typeof(char?), handler);
            handler = new NullableDateTimeTypeHandler();
            this.Register(typeof(DateTime?), handler);
            handler = new NullableDecimalTypeHandler();
            this.Register(typeof(decimal?), handler);
            handler = new NullableDoubleTypeHandler();
            this.Register(typeof(double?), handler);
            handler = new NullableGuidTypeHandler();
            this.Register(typeof(Guid?), handler);
            handler = new NullableInt16TypeHandler();
            this.Register(typeof(short?), handler);
            handler = new NullableInt32TypeHandler();
            this.Register(typeof(int?), handler);
            handler = new NullableInt64TypeHandler();
            this.Register(typeof(long?), handler);
            handler = new NullableSingleTypeHandler();
            this.Register(typeof(float?), handler);
            handler = new NullableUInt16TypeHandler();
            this.Register(typeof(ushort?), handler);
            handler = new NullableUInt32TypeHandler();
            this.Register(typeof(uint?), handler);
            handler = new NullableUInt64TypeHandler();
            this.Register(typeof(ulong?), handler);
            handler = new NullableSByteTypeHandler();
            this.Register(typeof(sbyte?), handler);
            handler = new NullableTimeSpanTypeHandler();
            this.Register(typeof(TimeSpan?), handler);
            this._unknownTypeHandler = new UnknownTypeHandler(this);
        }

        internal void AddTypeAlias(string key, TypeAlias typeAlias)
        {
            if (this._typeAliasMaps.Contains(key))
            {
                throw new DataMapperException(" Alias name conflict occurred.  The type alias '" + key + "' is already mapped to the value '" + typeAlias.ClassName + "'.");
            }
            this._typeAliasMaps.Add(key, typeAlias);
        }

        private ITypeHandler GetPrivateTypeHandler(Type type, string dbType)
        {
            IDictionary dictionary = (IDictionary) this._typeHandlerMap[type];
            ITypeHandler handler = null;
            if (dictionary != null)
            {
                if (dbType == null)
                {
                    handler = (ITypeHandler) dictionary["_NULL_TYPE_"];
                }
                else
                {
                    handler = (ITypeHandler) dictionary[dbType];
                    if (handler == null)
                    {
                        handler = (ITypeHandler) dictionary["_NULL_TYPE_"];
                    }
                }
                if (handler == null)
                {
                    throw new DataMapperException(string.Format("Type handler for {0} not registered.", type.Name));
                }
            }
            return handler;
        }

        internal Type GetType(string className)
        {
            TypeAlias typeAlias = this.GetTypeAlias(className);
            if (typeAlias != null)
            {
                return typeAlias.Class;
            }
            return TypeUtils.ResolveType(className);
        }

        internal TypeAlias GetTypeAlias(string name)
        {
            if (this._typeAliasMaps.Contains(name))
            {
                return (TypeAlias) this._typeAliasMaps[name];
            }
            return null;
        }

        public ITypeHandler GetTypeHandler(Type type)
        {
            return this.GetTypeHandler(type, null);
        }

        public ITypeHandler GetTypeHandler(Type type, string dbType)
        {
            if (type.IsEnum)
            {
                return this.GetPrivateTypeHandler(typeof(Enum), dbType);
            }
            return this.GetPrivateTypeHandler(type, dbType);
        }

        public ITypeHandler GetUnkownTypeHandler()
        {
            return this._unknownTypeHandler;
        }

        public bool IsSimpleType(Type type)
        {
            bool isSimpleType = false;
            if (type != null)
            {
                ITypeHandler typeHandler = this.GetTypeHandler(type, null);
                if (typeHandler != null)
                {
                    isSimpleType = typeHandler.IsSimpleType;
                }
            }
            return isSimpleType;
        }

        public void Register(Type type, ITypeHandler handler)
        {
            this.Register(type, null, handler);
        }

        public void Register(Type type, string dbType, ITypeHandler handler)
        {
            HybridDictionary dictionary = (HybridDictionary) this._typeHandlerMap[type];
            if (dictionary == null)
            {
                dictionary = new HybridDictionary();
                this._typeHandlerMap.Add(type, dictionary);
            }
            if (dbType == null)
            {
                if (_logger.IsInfoEnabled)
                {
                    ITypeHandler handler2 = (ITypeHandler) dictionary["_NULL_TYPE_"];
                    if (handler2 != null)
                    {
                        CustomTypeHandler handler3 = handler as CustomTypeHandler;
                        string str = string.Empty;
                        if (handler3 != null)
                        {
                            str = handler3.Callback.ToString();
                        }
                        else
                        {
                            str = handler.ToString();
                        }
                        _logger.Info("Replacing type handler [" + handler2.ToString() + "] with [" + str + "].");
                    }
                }
                dictionary["_NULL_TYPE_"] = handler;
            }
            else
            {
                dictionary.Add(dbType, handler);
            }
        }
    }
}

