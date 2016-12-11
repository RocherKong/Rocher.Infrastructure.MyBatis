namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isNotEqual", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsNotEqual : Conditional
    {
        public IsNotEqual(AccessorFactory accessorFactory)
        {
            base.Handler = new IsNotEqualTagHandler(accessorFactory);
        }
    }
}

