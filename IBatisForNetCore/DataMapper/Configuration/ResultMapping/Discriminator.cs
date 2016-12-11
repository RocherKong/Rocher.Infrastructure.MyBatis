namespace IBatisNet.DataMapper.Configuration.ResultMapping
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("discriminator", Namespace="http://ibatis.apache.org/mapping")]
    public class Discriminator
    {
        [NonSerialized]
        private string _callBackName = string.Empty;
        [NonSerialized]
        private string _clrType = string.Empty;
        [NonSerialized]
        private int _columnIndex = -999999;
        [NonSerialized]
        private string _columnName = string.Empty;
        [NonSerialized]
        private string _dbType = string.Empty;
        [NonSerialized]
        private IBatisNet.DataMapper.Configuration.ResultMapping.ResultProperty _mapping;
        [NonSerialized]
        private string _nullValue = string.Empty;
        [NonSerialized]
        private HybridDictionary _resultMaps = new HybridDictionary();
        [NonSerialized]
        private ArrayList _subMaps = new ArrayList();

        public void Add(SubMap subMap)
        {
            this._subMaps.Add(subMap);
        }

        public IResultMap GetSubMap(string discriminatorValue)
        {
            return (this._resultMaps[discriminatorValue] as ResultMap);
        }

        public void Initialize(ConfigurationScope configScope)
        {
            int count = this._subMaps.Count;
            for (int i = 0; i < count; i++)
            {
                SubMap map = this._subMaps[i] as SubMap;
                this._resultMaps.Add(map.DiscriminatorValue, configScope.SqlMapper.GetResultMap(map.ResultMapName));
            }
        }

        public void SetMapping(ConfigurationScope configScope, Type resultClass)
        {
            configScope.ErrorContext.MoreInfo = "Initialize discriminator mapping";
            this._mapping = new IBatisNet.DataMapper.Configuration.ResultMapping.ResultProperty();
            this._mapping.ColumnName = this._columnName;
            this._mapping.ColumnIndex = this._columnIndex;
            this._mapping.CLRType = this._clrType;
            this._mapping.CallBackName = this._callBackName;
            this._mapping.DbType = this._dbType;
            this._mapping.NullValue = this._nullValue;
            this._mapping.Initialize(configScope, resultClass);
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

        [XmlAttribute("columnIndex")]
        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
            set
            {
                this._columnIndex = value;
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

        [XmlIgnore]
        public IBatisNet.DataMapper.Configuration.ResultMapping.ResultProperty ResultProperty
        {
            get
            {
                return this._mapping;
            }
        }
    }
}

