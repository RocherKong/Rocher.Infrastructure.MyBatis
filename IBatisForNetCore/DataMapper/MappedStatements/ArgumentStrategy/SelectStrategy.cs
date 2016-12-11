namespace IBatisNet.DataMapper.MappedStatements.ArgumentStrategy
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public class SelectStrategy : IArgumentStrategy
    {
        private IArgumentStrategy _selectStrategy;

        public SelectStrategy(ResultProperty mapping, IArgumentStrategy selectArrayStrategy, IArgumentStrategy selectGenericListStrategy, IArgumentStrategy selectListStrategy, IArgumentStrategy selectObjectStrategy)
        {
            if (mapping.MemberType.BaseType == typeof(Array))
            {
                this._selectStrategy = selectArrayStrategy;
            }
            else if (mapping.MemberType.IsGenericType && typeof(IList<>).IsAssignableFrom(mapping.MemberType.GetGenericTypeDefinition()))
            {
                this._selectStrategy = selectGenericListStrategy;
            }
            else if (typeof(IList).IsAssignableFrom(mapping.MemberType))
            {
                this._selectStrategy = selectListStrategy;
            }
            else
            {
                this._selectStrategy = selectObjectStrategy;
            }
        }

        public object GetValue(RequestScope request, ResultProperty mapping, ref IDataReader reader, object selectKeys)
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
                return null;
            }
            return this._selectStrategy.GetValue(request, mapping, ref reader, keys);
        }
    }
}

