namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;

    public class IsEqualTagHandler : ConditionalTagHandler
    {
        public IsEqualTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
        {
        }

        public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            return (base.Compare(ctx, tag, parameterObject) == 0L);
        }
    }
}

