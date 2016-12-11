namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot("isPropertyAvailable", Namespace="http://ibatis.apache.org/mapping")]
    public sealed class IsPropertyAvailable : BaseTag
    {
        public IsPropertyAvailable(AccessorFactory accessorFactory)
        {
            base.Handler = new IsPropertyAvailableTagHandler(accessorFactory);
        }
    }
}

