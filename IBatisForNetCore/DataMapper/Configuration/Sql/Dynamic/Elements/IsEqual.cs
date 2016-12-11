namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isEqual", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsEqual : Conditional
    {
        public IsEqual(AccessorFactory accessorFactory)
        {
            base.Handler = new IsEqualTagHandler(accessorFactory);
        }
    }
}

