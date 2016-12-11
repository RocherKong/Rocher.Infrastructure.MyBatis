namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;
    using System.Collections;

    public class IsEmptyTagHandler : ConditionalTagHandler
    {
        public IsEmptyTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
        {
        }

        public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            if (parameterObject != null)
            {
                string property = ((BaseTag) tag).Property;
                object obj2 = null;
                if ((property != null) && (property.Length > 0))
                {
                    obj2 = ObjectProbe.GetMemberValue(parameterObject, property, base.AccessorFactory);
                }
                else
                {
                    obj2 = parameterObject;
                }
                if (obj2 is ICollection)
                {
                    if (obj2 != null)
                    {
                        return (((ICollection) obj2).Count < 1);
                    }
                    return true;
                }
                if ((obj2 != null) && typeof(Array).IsAssignableFrom(obj2.GetType()))
                {
                    return (((Array) obj2).GetLength(0) == 0);
                }
                if (obj2 != null)
                {
                    return Convert.ToString(obj2).Equals("");
                }
            }
            return true;
        }
    }
}

