namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Data;
    using System.Text;

    public abstract class BaseStrategy
    {
        private const string KEY_SEPARATOR = "\002";
        public static object SKIP = new object();

        protected BaseStrategy()
        {
        }

        protected bool FillObjectWithReaderAndResultMap(RequestScope request, IDataReader reader, IResultMap resultMap, ref object resultObject)
        {
            bool flag = false;
            if (resultMap.Properties.Count <= 0)
            {
                return true;
            }
            for (int i = 0; i < resultMap.Properties.Count; i++)
            {
                request.IsRowDataFound = false;
                ResultProperty mapping = resultMap.Properties[i];
                mapping.PropertyStrategy.Set(request, resultMap, mapping, ref resultObject, reader, null);
                flag = flag || request.IsRowDataFound;
            }
            request.IsRowDataFound = flag;
            return flag;
        }

        protected string GetUniqueKey(IResultMap resultMap, RequestScope request, IDataReader reader)
        {
            if (resultMap.GroupByProperties.Count <= 0)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < resultMap.GroupByProperties.Count; i++)
            {
                ResultProperty property = resultMap.GroupByProperties[i];
                builder.Append(property.GetDataBaseValue(reader));
                builder.Append('-');
            }
            if (builder.Length < 1)
            {
                return null;
            }
            builder.Append("\002");
            return builder.ToString();
        }
    }
}

