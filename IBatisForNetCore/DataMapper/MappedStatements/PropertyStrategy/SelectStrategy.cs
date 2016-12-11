namespace IBatisNet.DataMapper.MappedStatements.PropertyStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public class SelectStrategy : IPropertyStrategy
    {
        private IPropertyStrategy _selectStrategy;

        public SelectStrategy(ResultProperty mapping, IPropertyStrategy selectArrayStrategy, IPropertyStrategy selectGenericListStrategy, IPropertyStrategy selectListStrategy, IPropertyStrategy selectObjectStrategy)
        {
            if (mapping.SetAccessor.MemberType.BaseType == typeof(Array))
            {
                this._selectStrategy = selectArrayStrategy;
            }
            else if (mapping.SetAccessor.MemberType.IsGenericType && typeof(IList<>).IsAssignableFrom(mapping.SetAccessor.MemberType.GetGenericTypeDefinition()))
            {
                this._selectStrategy = selectGenericListStrategy;
            }
            else if (typeof(IList).IsAssignableFrom(mapping.SetAccessor.MemberType))
            {
                this._selectStrategy = selectListStrategy;
            }
            else
            {
                this._selectStrategy = selectObjectStrategy;
            }
        }

        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            throw new NotSupportedException("Get method on ResultMapStrategy is not supported");
        }

        public void Set(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader, object selectKeys)
        {
            string columnName = mapping.ColumnName;
            object keys = null;
            bool flag = false;
            if ((columnName.IndexOf(',') > 0) || (columnName.IndexOf('=') > 0))
            {
                IDictionary dictionary = new Hashtable();
                keys = dictionary;
                char[] separator = new char[] { '=', ',' };
                string[] strArray = columnName.Split(separator);
                if ((strArray.Length % 2) != 0)
                {
                    throw new DataMapperException("Invalid composite key string format in '" + mapping.PropertyName + ". It must be: property1=column1,property2=column2,...");
                }
                IEnumerator enumerator = strArray.GetEnumerator();
                while (!flag && enumerator.MoveNext())
                {
                    string key = ((string) enumerator.Current).Trim();
                    if (columnName.Contains("="))
                    {
                        enumerator.MoveNext();
                    }
                    object obj3 = reader.GetValue(reader.GetOrdinal(((string) enumerator.Current).Trim()));
                    dictionary.Add(key, obj3);
                    flag = obj3 == DBNull.Value;
                }
            }
            else
            {
                keys = reader.GetValue(reader.GetOrdinal(columnName));
                flag = reader.IsDBNull(reader.GetOrdinal(columnName));
            }
            if (flag)
            {
                mapping.SetAccessor.Set(target, null);
            }
            else
            {
                this._selectStrategy.Set(request, resultMap, mapping, ref target, reader, keys);
            }
        }
    }
}

