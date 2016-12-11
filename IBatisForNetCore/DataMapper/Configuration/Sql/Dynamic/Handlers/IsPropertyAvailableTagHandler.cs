namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;

    public class IsPropertyAvailableTagHandler : ConditionalTagHandler
    {
        public IsPropertyAvailableTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
        {
        }

        public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            if (parameterObject == null)
            {
                return false;
            }
            return ObjectProbe.HasReadableProperty(parameterObject, ((BaseTag) tag).Property);
        }
    }
}

