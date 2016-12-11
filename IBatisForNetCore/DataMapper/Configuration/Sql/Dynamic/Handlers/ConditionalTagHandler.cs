namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Exceptions;
    using System;
    using System.Text;

    public abstract class ConditionalTagHandler : BaseTagHandler
    {
        public const long NOT_COMPARABLE = -9223372036854775808L;

        public ConditionalTagHandler(AccessorFactory accessorFactory) : base(accessorFactory)
        {
        }

        protected long Compare(SqlTagContext ctx, SqlTag sqlTag, object parameterObject)
        {
            Conditional conditional = (Conditional) sqlTag;
            string property = conditional.Property;
            string compareProperty = conditional.CompareProperty;
            string compareValue = conditional.CompareValue;
            object obj2 = null;
            Type type = null;
            if ((property != null) && (property.Length > 0))
            {
                obj2 = ObjectProbe.GetMemberValue(parameterObject, property, base.AccessorFactory);
                type = obj2.GetType();
            }
            else
            {
                obj2 = parameterObject;
                if (obj2 != null)
                {
                    type = parameterObject.GetType();
                }
                else
                {
                    type = typeof(object);
                }
            }
            if ((compareProperty != null) && (compareProperty.Length > 0))
            {
                object obj3 = ObjectProbe.GetMemberValue(parameterObject, compareProperty, base.AccessorFactory);
                return this.CompareValues(type, obj2, obj3);
            }
            switch (compareValue)
            {
                case null:
                case "":
                    throw new DataMapperException("Error comparing in conditional fragment.  Uknown 'compare to' values.");
            }
            return this.CompareValues(type, obj2, compareValue);
        }

        protected long CompareValues(Type type, object value1, object value2)
        {
            if ((value1 == null) || (value2 == null))
            {
                return ((value1 == value2) ? 0L : -9223372036854775808L);
            }
            if (value2.GetType() != type)
            {
                value2 = this.ConvertValue(type, value2.ToString());
            }
            if ((value2 is string) && (type != typeof(string)))
            {
                value1 = value1.ToString();
            }
            if (!(value1 is IComparable) || !(value2 is IComparable))
            {
                value1 = value1.ToString();
                value2 = value2.ToString();
            }
            return ((IComparable) value1).CompareTo(value2);
        }

        protected object ConvertValue(Type type, string value)
        {
            if (type != typeof(string))
            {
                if (type == typeof(bool))
                {
                    return Convert.ToBoolean(value);
                }
                if (type == typeof(byte))
                {
                    return Convert.ToByte(value);
                }
                if (type == typeof(char))
                {
                    return Convert.ToChar(value.Substring(0, 1));
                }
                if (type == typeof(DateTime))
                {
                    try
                    {
                        return Convert.ToDateTime(value);
                    }
                    catch (Exception exception)
                    {
                        throw new DataMapperException("Error parsing date. Cause: " + exception.Message, exception);
                    }
                }
                if (type == typeof(decimal))
                {
                    return Convert.ToDecimal(value);
                }
                if (type == typeof(double))
                {
                    return Convert.ToDouble(value);
                }
                if (type == typeof(short))
                {
                    return Convert.ToInt16(value);
                }
                if (type == typeof(int))
                {
                    return Convert.ToInt32(value);
                }
                if (type == typeof(long))
                {
                    return Convert.ToInt64(value);
                }
                if (type == typeof(float))
                {
                    return Convert.ToSingle(value);
                }
            }
            return value;
        }

        public override int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, StringBuilder bodyContent)
        {
            return 1;
        }

        public override int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            if (this.IsCondition(ctx, tag, parameterObject))
            {
                return 1;
            }
            return 0;
        }

        public abstract bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject);
    }
}

