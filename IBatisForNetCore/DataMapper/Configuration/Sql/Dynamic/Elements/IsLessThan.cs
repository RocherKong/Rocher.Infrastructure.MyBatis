namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isLessThan", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsLessThan : Conditional
    {
        public IsLessThan(AccessorFactory accessorFactory)
        {
            base.Handler = new IsLessThanTagHandler(accessorFactory);
        }
    }
}

