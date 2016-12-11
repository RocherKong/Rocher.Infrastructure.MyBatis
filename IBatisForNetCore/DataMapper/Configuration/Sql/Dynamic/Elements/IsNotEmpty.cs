namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isNotEmpty", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsNotEmpty : BaseTag
    {
        public IsNotEmpty(AccessorFactory accessorFactory)
        {
            base.Handler = new IsNotEmptyTagHandler(accessorFactory);
        }
    }
}

