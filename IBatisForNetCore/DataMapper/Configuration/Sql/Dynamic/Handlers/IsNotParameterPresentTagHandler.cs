namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;

    public sealed class IsNotParameterPresentTagHandler : IsParameterPresentTagHandler
    {
        public IsNotParameterPresentTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
        {
        }

        public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            return !base.IsCondition(ctx, tag, parameterObject);
        }
    }
}

