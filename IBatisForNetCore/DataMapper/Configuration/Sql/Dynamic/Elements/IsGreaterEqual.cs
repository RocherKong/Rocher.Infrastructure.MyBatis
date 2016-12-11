namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isGreaterEqual", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsGreaterEqual : Conditional
    {
        public IsGreaterEqual(AccessorFactory accessorFactory)
        {
            base.Handler = new IsGreaterEqualTagHandler(accessorFactory);
        }
    }
}

