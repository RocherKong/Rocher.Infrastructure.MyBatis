namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.Configuration;
    using IBatisNet.DataMapper.Configuration.Serializers;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("resultMap", Namespace="http://ibatis.apache.org/mapping")]
    public class ResultMap : IResultMap
    {
        [NonSerialized]
        private Type _class;
        [NonSerialized]
        private string _className = string.Empty;
        [NonSerialized]
        private IDataExchange _dataExchange;
        [NonSerialized]
        private DataExchangeFactory _dataExchangeFactory;
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.ResultMapping.Discriminator _discriminator;
        [NonSerialized]
        private string _extendMap = string.Empty;
        [NonSerialized]
        private ResultPropertyCollection _groupByProperties = new ResultPropertyCollection();
        [NonSerialized]
        private StringCollection _groupByPropertyNames = new StringCollection();
        [NonSerialized]
        private string _id = string.Empty;
        [NonSerialized]
        private bool _isInitalized = true;
        private static IResultMap _nullResultMap = null;
        [NonSerialized]
        private IFactory _objectFactory;
        [NonSerialized]
        private ResultPropertyCollection _parameters = new ResultPropertyCollection();
        [NonSerialized]
        private ResultPropertyCollection _properties = new ResultPropertyCollection();
        [NonSerialized]
        private string _sqlMapNameSpace = string.Empty;
        public static BindingFlags ANY_VISIBILITY_INSTANCE = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        private const string XML_CONSTRUCTOR_ARGUMENT = "constructor/argument";
        private const string XML_DISCRIMNATOR = "discriminator";
        private const string XML_RESULT = "result";
        private const string XML_SUBMAP = "subMap";

        public ResultMap(ConfigurationScope configScope, string id, string className, string extendMap, string groupBy)
        {
            _nullResultMap = new NullResultMap();
            this._dataExchangeFactory = configScope.DataExchangeFactory;
            this._sqlMapNameSpace = configScope.SqlMapNamespace;
            if ((id == null) || (id.Length < 1))
            {
                throw new ArgumentNullException("The id attribute is mandatory in a ResultMap tag.");
            }
            this._id = configScope.ApplyNamespace(id);
            if ((className == null) || (className.Length < 1))
            {
                throw new ArgumentNullException("The class attribute is mandatory in the ResultMap tag id:" + this._id);
            }
            this._className = className;
            this._extendMap = extendMap;
            if ((groupBy != null) && (groupBy.Length > 0))
            {
                string[] strArray = groupBy.Split(new char[] { ',' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str = strArray[i].Trim();
                    this._groupByPropertyNames.Add(str);
                }
            }
        }

        public object CreateInstanceOfResult(object[] parameters)
        {
            TypeCode typeCode = Type.GetTypeCode(this._class);
            if (typeCode == TypeCode.Object)
            {
                return this._objectFactory.CreateInstance(parameters);
            }
            return TypeUtils.InstantiatePrimitiveType(typeCode);
        }

        private void GetChildNode(ConfigurationScope configScope)
        {
            ResultProperty property = null;
            SubMap subMap = null;
            XmlNodeList list = configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("constructor/argument"), configScope.XmlNamespaceManager);
            if (list.Count > 0)
            {
                Type[] types = new Type[list.Count];
                string[] parametersName = new string[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    ArgumentProperty property2 = ArgumentPropertyDeSerializer.Deserialize(list[i], configScope);
                    this._parameters.Add(property2);
                    parametersName[i] = property2.ArgumentName;
                }
                ConstructorInfo constructor = this.GetConstructor(this._class, parametersName);
                for (int j = 0; j < this._parameters.Count; j++)
                {
                    ArgumentProperty property3 = (ArgumentProperty) this._parameters[j];
                    configScope.ErrorContext.MoreInfo = "initialize argument property : " + property3.ArgumentName;
                    property3.Initialize(configScope, constructor);
                    types[j] = property3.MemberType;
                }
                this._objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(this._class, types);
            }
            else if (Type.GetTypeCode(this._class) == TypeCode.Object)
            {
                this._objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(this._class, Type.EmptyTypes);
            }
            foreach (XmlNode node in configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("result"), configScope.XmlNamespaceManager))
            {
                property = ResultPropertyDeSerializer.Deserialize(node, configScope);
                configScope.ErrorContext.MoreInfo = "initialize result property: " + property.PropertyName;
                property.Initialize(configScope, this._class);
                this._properties.Add(property);
            }
            XmlNode node2 = configScope.NodeContext.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix("discriminator"), configScope.XmlNamespaceManager);
            if (node2 != null)
            {
                configScope.ErrorContext.MoreInfo = "initialize discriminator";
                this.Discriminator = DiscriminatorDeSerializer.Deserialize(node2, configScope);
                this.Discriminator.SetMapping(configScope, this._class);
            }
            if ((configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("subMap"), configScope.XmlNamespaceManager).Count > 0) && (this.Discriminator == null))
            {
                throw new ConfigurationException("The discriminator is null, but somehow a subMap was reached.  This is a bug.");
            }
            foreach (XmlNode node3 in configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("subMap"), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "initialize subMap";
                subMap = SubMapDeSerializer.Deserialize(node3, configScope);
                this.Discriminator.Add(subMap);
            }
        }

        private ConstructorInfo GetConstructor(Type type, string[] parametersName)
        {
            foreach (ConstructorInfo info in type.GetConstructors(ANY_VISIBILITY_INSTANCE))
            {
                ParameterInfo[] parameters = info.GetParameters();
                if (parameters.Length == parametersName.Length)
                {
                    bool flag = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (!(parameters[i].Name == parametersName[i]))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return info;
                    }
                }
            }
            throw new DataMapperException("Cannot find an appropriate constructor which map parameters in class: " + type.Name);
        }

        public void Initialize(ConfigurationScope configScope)
        {
            try
            {
                this._class = configScope.SqlMapper.TypeHandlerFactory.GetType(this._className);
                this._dataExchange = this._dataExchangeFactory.GetDataExchangeForClass(this._class);
                this.GetChildNode(configScope);
                for (int i = 0; i < this._groupByProperties.Count; i++)
                {
                    string propertyName = this.GroupByPropertyNames[i];
                    if (!this._properties.Contains(propertyName))
                    {
                        throw new ConfigurationException(string.Format("Could not configure ResultMap named \"{0}\". Check the groupBy attribute. Cause: there's no result property named \"{1}\".", this._id, propertyName));
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ConfigurationException(string.Format("Could not configure ResultMap named \"{0}\", Cause: {1}", this._id, exception.Message), exception);
            }
        }

        public void InitializeGroupByProperties()
        {
            for (int i = 0; i < this.GroupByPropertyNames.Count; i++)
            {
                ResultProperty property = this.Properties.FindByPropertyName(this.GroupByPropertyNames[i]);
                this.GroupByProperties.Add(property);
            }
        }

        public IResultMap ResolveSubMap(IDataReader dataReader)
        {
            IResultMap subMap = this;
            if (this._discriminator == null)
            {
                return subMap;
            }
            object dataBaseValue = this._discriminator.ResultProperty.GetDataBaseValue(dataReader);
            if (dataBaseValue != null)
            {
                subMap = this._discriminator.GetSubMap(dataBaseValue.ToString());
                if (subMap == null)
                {
                    return this;
                }
                if (subMap != this)
                {
                    subMap = subMap.ResolveSubMap(dataReader);
                }
                return subMap;
            }
            return _nullResultMap;
        }

        public void SetObjectFactory(ConfigurationScope configScope)
        {
            Type[] types = new Type[this._parameters.Count];
            for (int i = 0; i < this._parameters.Count; i++)
            {
                ArgumentProperty property = (ArgumentProperty) this._parameters[i];
                types[i] = property.MemberType;
            }
            this._objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(this._class, types);
        }

        public void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue)
        {
            this._dataExchange.SetData(ref target, property, dataBaseValue);
        }

        [XmlIgnore]
        public Type Class
        {
            get
            {
                return this._class;
            }
        }

        [XmlIgnore]
        public IDataExchange DataExchange
        {
            set
            {
                this._dataExchange = value;
            }
        }

        [XmlIgnore]
        public IBatisNet.DataMapper.Configuration.ResultMapping.Discriminator Discriminator
        {
            get
            {
                return this._discriminator;
            }
            set
            {
                this._discriminator = value;
            }
        }

        [XmlAttribute("extends")]
        public string ExtendMap
        {
            get
            {
                return this._extendMap;
            }
            set
            {
                this._extendMap = value;
            }
        }

        [XmlIgnore]
        public ResultPropertyCollection GroupByProperties
        {
            get
            {
                return this._groupByProperties;
            }
        }

        [XmlIgnore]
        public StringCollection GroupByPropertyNames
        {
            get
            {
                return this._groupByPropertyNames;
            }
        }

        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return this._id;
            }
        }

        public bool IsInitalized
        {
            get
            {
                return true;
            }
            set
            {
                this._isInitalized = value;
            }
        }

        [XmlIgnore]
        public ResultPropertyCollection Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        [XmlIgnore]
        public ResultPropertyCollection Properties
        {
            get
            {
                return this._properties;
            }
        }
    }
}

