namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using System;
    using System.Text;

    public sealed class IterateTagHandler : BaseTagHandler
    {
        public IterateTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
        {
        }

        public override int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, StringBuilder bodyContent)
        {
            IterateContext attribute = (IterateContext) ctx.GetAttribute(tag);
            if (!attribute.MoveNext())
            {
                return 1;
            }
            string property = ((BaseTag) tag).Property;
            if (property == null)
            {
                property = "";
            }
            string find = property + "[]";
            string replace = string.Concat(new object[] { property, "[", attribute.Index, "]" });
            Replace(bodyContent, find, replace);
            if (attribute.IsFirst)
            {
                string open = ((Iterate) tag).Open;
                if (open != null)
                {
                    bodyContent.Insert(0, open);
                    bodyContent.Insert(0, ' ');
                }
            }
            if (!attribute.IsLast)
            {
                string conjunction = ((Iterate) tag).Conjunction;
                if (conjunction != null)
                {
                    bodyContent.Append(conjunction);
                    bodyContent.Append(' ');
                }
            }
            if (attribute.IsLast)
            {
                string close = ((Iterate) tag).Close;
                if (close != null)
                {
                    bodyContent.Append(close);
                }
            }
            return 2;
        }

        public override void DoPrepend(SqlTagContext ctx, SqlTag tag, object parameterObject, StringBuilder bodyContent)
        {
            IterateContext attribute = (IterateContext) ctx.GetAttribute(tag);
            if (attribute.IsFirst)
            {
                base.DoPrepend(ctx, tag, parameterObject, bodyContent);
            }
        }

        public override int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            IterateContext attribute = (IterateContext) ctx.GetAttribute(tag);
            if (attribute == null)
            {
                object obj2;
                string property = ((BaseTag) tag).Property;
                if ((property != null) && (property.Length > 0))
                {
                    obj2 = ObjectProbe.GetMemberValue(parameterObject, property, base.AccessorFactory);
                }
                else
                {
                    obj2 = parameterObject;
                }
                attribute = new IterateContext(obj2);
                ctx.AddAttribute(tag, attribute);
            }
            if ((attribute != null) && attribute.HasNext)
            {
                return 1;
            }
            return 0;
        }

        private static void Replace(StringBuilder buffer, string find, string replace)
        {
            int index = buffer.ToString().IndexOf(find);
            int length = find.Length;
            while (index > -1)
            {
                buffer = buffer.Replace(find, replace, index, length);
                index = buffer.ToString().IndexOf(find);
            }
        }

        public override bool IsPostParseRequired
        {
            get
            {
                return true;
            }
        }
    }
}

