namespace IBatisNet.Common
{
    using IBatisNet.Common.Xml;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class ProviderDeSerializer
    {
        public static IDbProvider Deserialize(XmlNode node)
        {
            IDbProvider provider = new DbProvider();
            NameValueCollection attributes = NodeUtils.ParseAttributes(node);
            provider.AssemblyName = attributes["assemblyName"];
            provider.CommandBuilderClass = attributes["commandBuilderClass"];
            provider.DbCommandClass = attributes["commandClass"];
            provider.DbConnectionClass = attributes["connectionClass"];
            provider.DataAdapterClass = attributes["dataAdapterClass"];
            provider.Description = attributes["description"];
            provider.IsDefault = NodeUtils.GetBooleanAttribute(attributes, "default", false);
            provider.IsEnabled = NodeUtils.GetBooleanAttribute(attributes, "enabled", true);
            provider.Name = attributes["name"];
            provider.ParameterDbTypeClass = attributes["parameterDbTypeClass"];
            provider.ParameterDbTypeProperty = attributes["parameterDbTypeProperty"];
            provider.ParameterPrefix = attributes["parameterPrefix"];
            provider.SetDbParameterPrecision = NodeUtils.GetBooleanAttribute(attributes, "setDbParameterPrecision", true);
            provider.SetDbParameterScale = NodeUtils.GetBooleanAttribute(attributes, "setDbParameterScale", true);
            provider.SetDbParameterSize = NodeUtils.GetBooleanAttribute(attributes, "setDbParameterSize", true);
            provider.UseDeriveParameters = NodeUtils.GetBooleanAttribute(attributes, "useDeriveParameters", true);
            provider.UseParameterPrefixInParameter = NodeUtils.GetBooleanAttribute(attributes, "useParameterPrefixInParameter", true);
            provider.UseParameterPrefixInSql = NodeUtils.GetBooleanAttribute(attributes, "useParameterPrefixInSql", true);
            provider.UsePositionalParameters = NodeUtils.GetBooleanAttribute(attributes, "usePositionalParameters", false);
            provider.AllowMARS = NodeUtils.GetBooleanAttribute(attributes, "allowMARS", false);
            return provider;
        }
    }
}

