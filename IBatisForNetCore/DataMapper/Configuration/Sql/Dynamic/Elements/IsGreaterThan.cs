namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isGreaterThan", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsGreaterThan : Conditional
    {
        public IsGreaterThan(AccessorFactory accessorFactory)
        {
            base.Handler = new IsGreaterThanTagHandler(accessorFactory);
        }
    }
}

