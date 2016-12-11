namespace IBatisNet.DataMapper.Configuration.Statements
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("selectKey", Namespace="http://ibatis.apache.org/mapping")]
    public class SelectKey : Statement
    {
        [NonSerialized]
        private string _property = string.Empty;
        [NonSerialized]
        private IBatisNet.DataMapper.SelectKeyType _selectKeyType = IBatisNet.DataMapper.SelectKeyType.post;

        internal override void Initialize(ConfigurationScope configurationScope)
        {
            if (this.PropertyName.Length > 0)
            {
                IMappedStatement mappedStatement = configurationScope.SqlMapper.GetMappedStatement(base.Id);
                Type parameterClass = mappedStatement.Statement.ParameterClass;
                if ((parameterClass != null) && !ObjectProbe.IsSimpleType(parameterClass))
                {
                    configurationScope.ErrorContext.MoreInfo = string.Format("Looking for settable property named '{0}' on type '{1}' for selectKey node of statement id '{2}'.", this.PropertyName, mappedStatement.Statement.ParameterClass.Name, base.Id);
                    ReflectionInfo.GetInstance(mappedStatement.Statement.ParameterClass).GetSetter(this.PropertyName);
                }
            }
            base.Initialize(configurationScope);
        }

        [XmlIgnore]
        public override string ExtendStatement
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        [XmlIgnore]
        public bool isAfter
        {
            get
            {
                return (this._selectKeyType == IBatisNet.DataMapper.SelectKeyType.post);
            }
        }

        [XmlAttribute("property")]
        public string PropertyName
        {
            get
            {
                return this._property;
            }
            set
            {
                this._property = value;
            }
        }

        [XmlAttribute("type")]
        public IBatisNet.DataMapper.SelectKeyType SelectKeyType
        {
            get
            {
                return this._selectKeyType;
            }
            set
            {
                this._selectKeyType = value;
            }
        }
    }
}

