namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.MappedStatements.PropertyStrategy;
    using System;
    using System.Collections;
    using System.Data;

    public sealed class ReaderAutoMapper
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ResultPropertyCollection Build(DataExchangeFactory dataExchangeFactory, IDataReader reader, ref object resultObject)
        {
            Type type = resultObject.GetType();
            ResultPropertyCollection propertys = new ResultPropertyCollection();
            try
            {
                string[] writeableMemberNames = ReflectionInfo.GetInstance(type).GetWriteableMemberNames();
                Hashtable hashtable = new Hashtable();
                int length = writeableMemberNames.Length;
                for (int i = 0; i < length; i++)
                {
                    ISetAccessor accessor = dataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(type, writeableMemberNames[i]);
                    hashtable.Add(writeableMemberNames[i], accessor);
                }
                DataTable schemaTable = reader.GetSchemaTable();
                int count = schemaTable.Rows.Count;
                for (int j = 0; j < count; j++)
                {
                    string memberName = schemaTable.Rows[j][0].ToString();
                    ISetAccessor setAccessor = hashtable[memberName] as ISetAccessor;
                    ResultProperty property = new ResultProperty {
                        ColumnName = memberName,
                        ColumnIndex = j
                    };
                    if (resultObject is Hashtable)
                    {
                        property.PropertyName = memberName;
                        propertys.Add(property);
                    }
                    Type memberTypeForSetter = null;
                    if (setAccessor == null)
                    {
                        try
                        {
                            memberTypeForSetter = ObjectProbe.GetMemberTypeForSetter(resultObject, memberName);
                        }
                        catch
                        {
                            _logger.Error("The column [" + memberName + "] could not be auto mapped to a property on [" + resultObject.ToString() + "]");
                        }
                    }
                    else
                    {
                        memberTypeForSetter = setAccessor.MemberType;
                    }
                    if ((memberTypeForSetter != null) || (setAccessor != null))
                    {
                        property.PropertyName = (setAccessor != null) ? setAccessor.Name : memberName;
                        if (setAccessor != null)
                        {
                            property.Initialize(dataExchangeFactory.TypeHandlerFactory, setAccessor);
                        }
                        else
                        {
                            property.TypeHandler = dataExchangeFactory.TypeHandlerFactory.GetTypeHandler(memberTypeForSetter);
                        }
                        property.PropertyStrategy = PropertyStrategyFactory.Get(property);
                        propertys.Add(property);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new DataMapperException("Error automapping columns. Cause: " + exception.Message, exception);
            }
            return propertys;
        }
    }
}

