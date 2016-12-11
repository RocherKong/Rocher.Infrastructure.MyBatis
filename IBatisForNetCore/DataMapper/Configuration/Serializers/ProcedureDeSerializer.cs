namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class ProcedureDeSerializer
    {
        public static Procedure Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Procedure procedure = new Procedure();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            procedure.CacheModelName = NodeUtils.GetStringAttribute(attributes, "cacheModel");
            procedure.Id = NodeUtils.GetStringAttribute(attributes, "id");
            procedure.ListClassName = NodeUtils.GetStringAttribute(attributes, "listClass");
            procedure.ParameterMapName = NodeUtils.GetStringAttribute(attributes, "parameterMap");
            procedure.ResultClassName = NodeUtils.GetStringAttribute(attributes, "resultClass");
            procedure.ResultMapName = NodeUtils.GetStringAttribute(attributes, "resultMap");
            procedure.ListClassName = NodeUtils.GetStringAttribute(attributes, "listClass");
            return procedure;
        }
    }
}

