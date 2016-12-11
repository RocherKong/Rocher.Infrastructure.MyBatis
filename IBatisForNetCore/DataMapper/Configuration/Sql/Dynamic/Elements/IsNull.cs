namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isNull", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsNull : BaseTag
    {
        public IsNull(AccessorFactory accessorFactory)
        {
            base.Handler = new IsNullTagHandler(accessorFactory);
        }
    }
}

