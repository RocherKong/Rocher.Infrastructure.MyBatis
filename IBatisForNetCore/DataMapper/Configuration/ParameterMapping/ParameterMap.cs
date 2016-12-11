namespace IBatisNet.DataMapper.Configuration.ParameterMapping
{
    using IBatisNet.Common.Logging;
    using IBatisNet.DataMapper.Configuration;
    using IBatisNet.DataMapper.Configuration.Serializers;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Data;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("parameterMap", Namespace="http://ibatis.apache.org/mapping")]
    public class ParameterMap
    {
        [NonSerialized]
        private string _className = string.Empty;
        [NonSerialized]
        private IDataExchange _dataExchange;
        [NonSerialized]
        private DataExchangeFactory _dataExchangeFactory;
        [NonSerialized]
        private string _extendMap = string.Empty;
        [NonSerialized]
        private string _id = string.Empty;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        [NonSerialized]
        private Type _parameterClass;
        [NonSerialized]
        private ParameterPropertyCollection _properties = new ParameterPropertyCollection();
        [NonSerialized]
        private ParameterPropertyCollection _propertiesList = new ParameterPropertyCollection();
        [NonSerialized]
        private Hashtable _propertiesMap = new Hashtable();
        [NonSerialized]
        private bool _usePositionalParameters;
        private const string XML_PARAMATER = "parameter";

        public ParameterMap(DataExchangeFactory dataExchangeFactory)
        {
            this._dataExchangeFactory = dataExchangeFactory;
        }

        public void AddParameterProperty(ParameterProperty property)
        {
            this._propertiesMap[property.PropertyName] = property;
            this._properties.Add(property);
            if (!this._propertiesList.Contains(property))
            {
                this._propertiesList.Add(property);
            }
        }

        public void BuildProperties(ConfigurationScope configScope)
        {
            ParameterProperty property = null;
            foreach (XmlNode node in configScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix("parameter"), configScope.XmlNamespaceManager))
            {
                property = ParameterPropertyDeSerializer.Deserialize(node, configScope);
                property.Initialize(configScope, this._parameterClass);
                this.AddParameterProperty(property);
            }
        }

        public int GetParameterIndex(string propertyName)
        {
            return Convert.ToInt32(propertyName.Replace("[", "").Replace("]", ""));
        }

        public ParameterProperty GetProperty(int index)
        {
            if (this._usePositionalParameters)
            {
                return this._properties[index];
            }
            return this._propertiesList[index];
        }

        public ParameterProperty GetProperty(string name)
        {
            return (ParameterProperty) this._propertiesMap[name];
        }

        public string[] GetPropertyNameArray()
        {
            string[] strArray = new string[this._propertiesMap.Count];
            for (int i = 0; i < this._propertiesList.Count; i++)
            {
                strArray[i] = this._propertiesList[i].PropertyName;
            }
            return strArray;
        }

        public void Initialize(bool usePositionalParameters, IScope scope)
        {
            this._usePositionalParameters = usePositionalParameters;
            if (this._className.Length > 0)
            {
                this._parameterClass = this._dataExchangeFactory.TypeHandlerFactory.GetType(this._className);
                this._dataExchange = this._dataExchangeFactory.GetDataExchangeForClass(this._parameterClass);
            }
            else
            {
                this._dataExchange = this._dataExchangeFactory.GetDataExchangeForClass(null);
            }
        }

        public void InsertParameterProperty(int index, ParameterProperty property)
        {
            this._propertiesMap[property.PropertyName] = property;
            this._properties.Insert(index, property);
            if (!this._propertiesList.Contains(property))
            {
                this._propertiesList.Insert(index, property);
            }
        }

        public void SetOutputParameter(ref object target, ParameterProperty mapping, object dataBaseValue)
        {
            this._dataExchange.SetData(ref target, mapping, dataBaseValue);
        }

        public void SetParameter(ParameterProperty mapping, IDataParameter dataParameter, object parameterValue)
        {
            object data = this._dataExchange.GetData(mapping, parameterValue);
            ITypeHandler typeHandler = mapping.TypeHandler;
            if (mapping.HasNullValue && typeHandler.Equals(data, mapping.NullValue))
            {
                data = null;
            }
            typeHandler.SetParameter(dataParameter, data, mapping.DbType);
        }

        [XmlIgnore]
        public Type Class
        {
            get
            {
                return this._parameterClass;
            }
            set
            {
                this._parameterClass = value;
            }
        }

        [XmlAttribute("class")]
        public string ClassName
        {
            get
            {
                return this._className;
            }
            set
            {
                if (_logger.IsInfoEnabled && ((value == null) || (value.Length < 1)))
                {
                    _logger.Info("The class attribute is recommended for better performance in a ParameterMap tag '" + this._id + "'.");
                }
                this._className = value;
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

        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return this._id;
            }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The id attribute is mandatory in a ParameterMap tag.");
                }
                this._id = value;
            }
        }

        [XmlIgnore]
        public ParameterPropertyCollection Properties
        {
            get
            {
                return this._properties;
            }
        }

        [XmlIgnore]
        public ParameterPropertyCollection PropertiesList
        {
            get
            {
                return this._propertiesList;
            }
        }
    }
}

