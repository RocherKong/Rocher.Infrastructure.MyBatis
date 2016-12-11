namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isParameterPresent", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsParameterPresent : SqlTag
    {
        public IsParameterPresent(AccessorFactory accessorFactory)
        {
            base.Handler = new IsParameterPresentTagHandler(accessorFactory);
        }
    }
}

