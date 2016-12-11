namespace IBatisNet.DataMapper.Configuration.Serializers
{
    using IBatisNet.Common.Xml;
    using IBatisNet.DataMapper.Configuration.Alias;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Xml;

    public sealed class TypeAliasDeSerializer
    {
        public static void Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            TypeAlias typeAlias = new TypeAlias();
            configScope.ErrorContext.MoreInfo = "loading type alias";
            NameValueCollection attributes = NodeUtils.ParseAttributes(node, configScope.Properties);
            typeAlias.Name = NodeUtils.GetStringAttribute(attributes, "alias");
            typeAlias.ClassName = NodeUtils.GetStringAttribute(attributes, "type");
            configScope.ErrorContext.ObjectId = typeAlias.ClassName;
            configScope.ErrorContext.MoreInfo = "initialize type alias";
            typeAlias.Initialize();
            configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
        }
    }
}

