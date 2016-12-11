namespace IBatisNet.DataMapper.Configuration.Statements
{
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("procedure", Namespace="http://ibatis.apache.org/mapping")]
    public class Procedure : Statement
    {
        internal override void Initialize(ConfigurationScope configurationScope)
        {
            base.Initialize(configurationScope);
            if (base.ParameterMap == null)
            {
                base.ParameterMap = configurationScope.SqlMapper.GetParameterMap("iBATIS.Empty.ParameterMap");
            }
        }

        [XmlIgnore]
        public override System.Data.CommandType CommandType
        {
            get
            {
                return System.Data.CommandType.StoredProcedure;
            }
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
    }
}

