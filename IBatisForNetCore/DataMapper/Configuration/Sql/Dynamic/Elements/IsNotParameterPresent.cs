namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isNotParameterPresent", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsNotParameterPresent : SqlTag
    {
        public IsNotParameterPresent(AccessorFactory accessorFactory)
        {
            base.Handler = new IsNotParameterPresentTagHandler(accessorFactory);
        }
    }
}

