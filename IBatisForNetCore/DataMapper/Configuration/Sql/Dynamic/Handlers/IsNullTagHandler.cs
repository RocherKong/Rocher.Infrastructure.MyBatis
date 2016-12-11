namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;

    public class IsNullTagHandler : ConditionalTagHandler
    {
        public IsNullTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
        {
        }

        public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            object obj2;
            if (parameterObject == null)
            {
                return true;
            }
            string property = ((BaseTag) tag).Property;
            if ((property != null) && (property.Length > 0))
            {
                obj2 = ObjectProbe.GetMemberValue(parameterObject, property, base.AccessorFactory);
            }
            else
            {
                obj2 = parameterObject;
            }
            return (obj2 == null);
        }
    }
}

