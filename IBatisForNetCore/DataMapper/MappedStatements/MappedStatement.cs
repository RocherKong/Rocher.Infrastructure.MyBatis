namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Commands;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.MappedStatements.PostSelectStrategy;
    using IBatisNet.DataMapper.MappedStatements.ResultStrategy;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class MappedStatement : IMappedStatement
    {
        private IPreparedCommand _preparedCommand;
        private IResultStrategy _resultStrategy;
        private ISqlMapper _sqlMap;
        private IStatement _statement;
        internal const int NO_MAXIMUM_RESULTS = -1;
        internal const int NO_SKIPPED_RESULTS = -1;

        public event ExecuteEventHandler Execute;

        internal MappedStatement(ISqlMapper sqlMap, IStatement statement)
        {
            this._sqlMap = sqlMap;
            this._statement = statement;
            this._preparedCommand = PreparedCommandFactory.GetPreparedCommand(false);
            this._resultStrategy = ResultStrategyFactory.Get(this._statement);
        }

        public virtual object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            object memberValue = null;
            SelectKey selectKey = null;
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            if (this._statement is Insert)
            {
                selectKey = ((Insert) this._statement).SelectKey;
            }
            if ((selectKey != null) && !selectKey.isAfter)
            {
                memberValue = this._sqlMap.GetMappedStatement(selectKey.Id).ExecuteQueryForObject(session, parameterObject);
                ObjectProbe.SetMemberValue(parameterObject, selectKey.PropertyName, memberValue, request.DataExchangeFactory.ObjectFactory, request.DataExchangeFactory.AccessorFactory);
            }
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            using (IDbCommand command = request.IDbCommand)
            {
                if (this._statement is Insert)
                {
                    command.ExecuteNonQuery();
                }
                else if (((this._statement is Procedure) && (this._statement.ResultClass != null)) && this._sqlMap.TypeHandlerFactory.IsSimpleType(this._statement.ResultClass))
                {
                    IDataParameter parameter = command.CreateParameter();
                    parameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(parameter);
                    command.ExecuteNonQuery();
                    memberValue = parameter.Value;
                    memberValue = this._sqlMap.TypeHandlerFactory.GetTypeHandler(this._statement.ResultClass).GetDataBaseValue(memberValue, this._statement.ResultClass);
                }
                else
                {
                    memberValue = command.ExecuteScalar();
                    if ((this._statement.ResultClass != null) && this._sqlMap.TypeHandlerFactory.IsSimpleType(this._statement.ResultClass))
                    {
                        memberValue = this._sqlMap.TypeHandlerFactory.GetTypeHandler(this._statement.ResultClass).GetDataBaseValue(memberValue, this._statement.ResultClass);
                    }
                }
                if ((selectKey != null) && selectKey.isAfter)
                {
                    memberValue = this._sqlMap.GetMappedStatement(selectKey.Id).ExecuteQueryForObject(session, parameterObject);
                    ObjectProbe.SetMemberValue(parameterObject, selectKey.PropertyName, memberValue, request.DataExchangeFactory.ObjectFactory, request.DataExchangeFactory.AccessorFactory);
                }
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            this.RaiseExecuteEvent();
            return memberValue;
        }

        private void ExecutePostSelect(RequestScope request)
        {
            while (request.QueueSelect.Count > 0)
            {
                PostBindind postSelect = request.QueueSelect.Dequeue() as PostBindind;
                PostSelectStrategyFactory.Get(postSelect.Method).Execute(postSelect, request);
            }
        }

        public virtual IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForDictionary<K, V>(request, session, parameterObject, keyProperty, valueProperty, null);
        }

        public virtual IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            if (rowDelegate == null)
            {
                throw new DataMapperException("A null DictionaryRowDelegate was passed to QueryForDictionary.");
            }
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForDictionary<K, V>(request, session, parameterObject, keyProperty, valueProperty, rowDelegate);
        }

        public virtual IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForList<T>(request, session, parameterObject, (IList<T>) null, (RowDelegate<T>) null);
        }

        public virtual IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForList(request, session, parameterObject, (IList) null, (RowDelegate) null);
        }

        public virtual void ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, IList<T> resultObject)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            this.RunQueryForList<T>(request, session, parameterObject, resultObject, null);
        }

        public virtual void ExecuteQueryForList(ISqlMapSession session, object parameterObject, IList resultObject)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            this.RunQueryForList(request, session, parameterObject, resultObject, null);
        }

        public virtual IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForList<T>(request, session, parameterObject, skipResults, maxResults);
        }

        public virtual IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForList(request, session, parameterObject, skipResults, maxResults);
        }

        public virtual IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForMap(request, session, parameterObject, keyProperty, valueProperty, null);
        }

        public virtual IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            if (rowDelegate == null)
            {
                throw new DataMapperException("A null DictionaryRowDelegate was passed to QueryForMapWithRowDelegate.");
            }
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForMap(request, session, parameterObject, keyProperty, valueProperty, rowDelegate);
        }

        public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForObject(session, parameterObject, null);
        }

        public virtual T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForObject<T>(session, parameterObject, default(T));
        }

        public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForObject(request, session, parameterObject, resultObject);
        }

        public virtual T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject, T resultObject)
        {
            T local = default(T);
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            return this.RunQueryForObject<T>(request, session, parameterObject, resultObject);
        }

        public virtual IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            if (rowDelegate == null)
            {
                throw new DataMapperException("A null RowDelegate was passed to QueryForRowDelegate.");
            }
            return this.RunQueryForList(request, session, parameterObject, null, rowDelegate);
        }

        public virtual IList<T> ExecuteQueryForRowDelegate<T>(ISqlMapSession session, object parameterObject, RowDelegate<T> rowDelegate)
        {
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            if (rowDelegate == null)
            {
                throw new DataMapperException("A null RowDelegate was passed to QueryForRowDelegate.");
            }
            return this.RunQueryForList<T>(request, session, parameterObject, null, rowDelegate);
        }

        public virtual int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            int num = 0;
            RequestScope request = this._statement.Sql.GetRequestScope(this, parameterObject, session);
            this._preparedCommand.Create(request, session, this.Statement, parameterObject);
            using (IDbCommand command = request.IDbCommand)
            {
                num = command.ExecuteNonQuery();
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            this.RaiseExecuteEvent();
            return num;
        }

        private void RaiseExecuteEvent()
        {
            ExecuteEventArgs e = new ExecuteEventArgs {
                StatementName = this._statement.Id
            };
            if (this.Execute != null)
            {
                this.Execute(this, e);
            }
        }

        private void RetrieveOutputParameters(RequestScope request, ISqlMapSession session, IDbCommand command, object result)
        {
            if (request.ParameterMap != null)
            {
                int count = request.ParameterMap.PropertiesList.Count;
                for (int i = 0; i < count; i++)
                {
                    ParameterProperty mapping = request.ParameterMap.GetProperty(i);
                    if ((mapping.Direction == ParameterDirection.Output) || (mapping.Direction == ParameterDirection.InputOutput))
                    {
                        string columnName = string.Empty;
                        if (!session.DataSource.DbProvider.UseParameterPrefixInParameter)
                        {
                            columnName = mapping.ColumnName;
                        }
                        else
                        {
                            columnName = session.DataSource.DbProvider.ParameterPrefix + mapping.ColumnName;
                        }
                        if (mapping.TypeHandler == null)
                        {
                            lock (mapping)
                            {
                                if (mapping.TypeHandler == null)
                                {
                                    Type memberTypeForGetter = ObjectProbe.GetMemberTypeForGetter(result, mapping.PropertyName);
                                    mapping.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(memberTypeForGetter);
                                }
                            }
                        }
                        IDataParameter parameter = (IDataParameter) command.Parameters[columnName];
                        object obj2 = parameter.Value;
                        object dataBaseValue = null;
                        if (obj2 == DBNull.Value)
                        {
                            if (mapping.HasNullValue)
                            {
                                dataBaseValue = mapping.TypeHandler.ValueOf(mapping.GetAccessor.MemberType, mapping.NullValue);
                            }
                            else
                            {
                                dataBaseValue = mapping.TypeHandler.NullValue;
                            }
                        }
                        else
                        {
                            dataBaseValue = mapping.TypeHandler.GetDataBaseValue(parameter.Value, result.GetType());
                        }
                        request.IsRowDataFound = request.IsRowDataFound || (dataBaseValue != null);
                        request.ParameterMap.SetOutputParameter(ref result, mapping, dataBaseValue);
                    }
                }
            }
        }

        internal IDictionary<K, V> RunQueryForDictionary<K, V>(RequestScope request, ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
        {
            IDictionary<K, V> dictionary = new Dictionary<K, V>();
            using (IDbCommand command = request.IDbCommand)
            {
                IDataReader reader = command.ExecuteReader();
                try
                {
                    object obj3;
                    if (rowDelegate != null)
                    {
                        goto Label_00F6;
                    }
                    while (reader.Read())
                    {
                        object obj2 = this._resultStrategy.Process(request, ref reader, null);
                        K local = (K) ObjectProbe.GetMemberValue(obj2, keyProperty, request.DataExchangeFactory.AccessorFactory);
                        V local2 = default(V);
                        if (valueProperty != null)
                        {
                            local2 = (V) ObjectProbe.GetMemberValue(obj2, valueProperty, request.DataExchangeFactory.AccessorFactory);
                        }
                        else
                        {
                            local2 = (V) obj2;
                        }
                        dictionary.Add(local, local2);
                    }
                    goto Label_0112;
                Label_008B:
                    obj3 = this._resultStrategy.Process(request, ref reader, null);
                    K key = (K) ObjectProbe.GetMemberValue(obj3, keyProperty, request.DataExchangeFactory.AccessorFactory);
                    V local4 = default(V);
                    if (valueProperty != null)
                    {
                        local4 = (V) ObjectProbe.GetMemberValue(obj3, valueProperty, request.DataExchangeFactory.AccessorFactory);
                    }
                    else
                    {
                        local4 = (V) obj3;
                    }
                    rowDelegate(key, local4, parameterObject, dictionary);
                Label_00F6:
                    if (reader.Read())
                    {
                        goto Label_008B;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
            Label_0112:
                this.ExecutePostSelect(request);
            }
            return dictionary;
        }

        internal IList<T> RunQueryForList<T>(RequestScope request, ISqlMapSession session, object parameterObject, IList<T> resultObject, RowDelegate<T> rowDelegate)
        {
            IList<T> list = resultObject;
            using (IDbCommand command = request.IDbCommand)
            {
                if (resultObject == null)
                {
                    if (this._statement.ListClass == null)
                    {
                        list = new List<T>();
                    }
                    else
                    {
                        list = this._statement.CreateInstanceOfGenericListClass<T>();
                    }
                }
                IDataReader reader = command.ExecuteReader();
                try
                {
                    T local;
                Label_0036:
                    if (rowDelegate != null)
                    {
                        goto Label_0090;
                    }
                    while (reader.Read())
                    {
                        object obj2 = this._resultStrategy.Process(request, ref reader, null);
                        if (obj2 != BaseStrategy.SKIP)
                        {
                            list.Add((T) obj2);
                        }
                    }
                    goto Label_0098;
                Label_006A:
                    local = (T) this._resultStrategy.Process(request, ref reader, null);
                    rowDelegate(local, parameterObject, list);
                Label_0090:
                    if (reader.Read())
                    {
                        goto Label_006A;
                    }
                Label_0098:
                    if (reader.NextResult())
                    {
                        goto Label_0036;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                this.ExecutePostSelect(request);
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            return list;
        }

        internal IList RunQueryForList(RequestScope request, ISqlMapSession session, object parameterObject, IList resultObject, RowDelegate rowDelegate)
        {
            IList list = resultObject;
            using (IDbCommand command = request.IDbCommand)
            {
                if (resultObject == null)
                {
                    if (this._statement.ListClass == null)
                    {
                        list = new ArrayList();
                    }
                    else
                    {
                        list = this._statement.CreateInstanceOfListClass();
                    }
                }
                IDataReader reader = command.ExecuteReader();
                try
                {
                    object obj3;
                Label_0036:
                    if (rowDelegate != null)
                    {
                        goto Label_0082;
                    }
                    while (reader.Read())
                    {
                        object obj2 = this._resultStrategy.Process(request, ref reader, null);
                        if (obj2 != BaseStrategy.SKIP)
                        {
                            list.Add(obj2);
                        }
                    }
                    goto Label_008A;
                Label_0066:
                    obj3 = this._resultStrategy.Process(request, ref reader, null);
                    rowDelegate(obj3, parameterObject, list);
                Label_0082:
                    if (reader.Read())
                    {
                        goto Label_0066;
                    }
                Label_008A:
                    if (reader.NextResult())
                    {
                        goto Label_0036;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                this.ExecutePostSelect(request);
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            return list;
        }

        internal IList<T> RunQueryForList<T>(RequestScope request, ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            IList<T> list = null;
            using (IDbCommand command = request.IDbCommand)
            {
                if (this._statement.ListClass == null)
                {
                    list = new List<T>();
                }
                else
                {
                    list = this._statement.CreateInstanceOfGenericListClass<T>();
                }
                IDataReader reader = command.ExecuteReader();
                try
                {
                    for (int i = 0; i < skipResults; i++)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                    }
                    for (int j = 0; ((maxResults == -1) || (j < maxResults)) && reader.Read(); j++)
                    {
                        object obj2 = this._resultStrategy.Process(request, ref reader, null);
                        if (obj2 != BaseStrategy.SKIP)
                        {
                            list.Add((T) obj2);
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                this.ExecutePostSelect(request);
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            return list;
        }

        internal IList RunQueryForList(RequestScope request, ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            IList list = null;
            using (IDbCommand command = request.IDbCommand)
            {
                if (this._statement.ListClass == null)
                {
                    list = new ArrayList();
                }
                else
                {
                    list = this._statement.CreateInstanceOfListClass();
                }
                IDataReader reader = command.ExecuteReader();
                try
                {
                    for (int i = 0; i < skipResults; i++)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                    }
                    for (int j = 0; ((maxResults == -1) || (j < maxResults)) && reader.Read(); j++)
                    {
                        object obj2 = this._resultStrategy.Process(request, ref reader, null);
                        if (obj2 != BaseStrategy.SKIP)
                        {
                            list.Add(obj2);
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                this.ExecutePostSelect(request);
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            return list;
        }

        internal IDictionary RunQueryForMap(RequestScope request, ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            IDictionary dictionary = new Hashtable();
            using (IDbCommand command = request.IDbCommand)
            {
                IDataReader reader = command.ExecuteReader();
                try
                {
                    object obj5;
                    if (rowDelegate != null)
                    {
                        goto Label_00C4;
                    }
                    while (reader.Read())
                    {
                        object obj2 = this._resultStrategy.Process(request, ref reader, null);
                        object obj3 = ObjectProbe.GetMemberValue(obj2, keyProperty, request.DataExchangeFactory.AccessorFactory);
                        object obj4 = obj2;
                        if (valueProperty != null)
                        {
                            obj4 = ObjectProbe.GetMemberValue(obj2, valueProperty, request.DataExchangeFactory.AccessorFactory);
                        }
                        dictionary.Add(obj3, obj4);
                    }
                    goto Label_00E0;
                Label_0072:
                    obj5 = this._resultStrategy.Process(request, ref reader, null);
                    object key = ObjectProbe.GetMemberValue(obj5, keyProperty, request.DataExchangeFactory.AccessorFactory);
                    object obj7 = obj5;
                    if (valueProperty != null)
                    {
                        obj7 = ObjectProbe.GetMemberValue(obj5, valueProperty, request.DataExchangeFactory.AccessorFactory);
                    }
                    rowDelegate(key, obj7, parameterObject, dictionary);
                Label_00C4:
                    if (reader.Read())
                    {
                        goto Label_0072;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
            Label_00E0:
                this.ExecutePostSelect(request);
            }
            return dictionary;
        }

        internal object RunQueryForObject(RequestScope request, ISqlMapSession session, object parameterObject, object resultObject)
        {
            object obj2 = resultObject;
            using (IDbCommand command = request.IDbCommand)
            {
                IDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        object obj3 = this._resultStrategy.Process(request, ref reader, resultObject);
                        if (obj3 != BaseStrategy.SKIP)
                        {
                            obj2 = obj3;
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                this.ExecutePostSelect(request);
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            this.RaiseExecuteEvent();
            return obj2;
        }

        internal T RunQueryForObject<T>(RequestScope request, ISqlMapSession session, object parameterObject, T resultObject)
        {
            T local = resultObject;
            using (IDbCommand command = request.IDbCommand)
            {
                IDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        object obj2 = this._resultStrategy.Process(request, ref reader, resultObject);
                        if (obj2 != BaseStrategy.SKIP)
                        {
                            local = (T) obj2;
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                this.ExecutePostSelect(request);
                this.RetrieveOutputParameters(request, session, command, parameterObject);
            }
            this.RaiseExecuteEvent();
            return local;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\tMappedStatement: " + this.Id);
            builder.Append(Environment.NewLine);
            if (this._statement.ParameterMap != null)
            {
                builder.Append(this._statement.ParameterMap.Id);
            }
            return builder.ToString();
        }

        public string Id
        {
            get
            {
                return this._statement.Id;
            }
        }

        public IPreparedCommand PreparedCommand
        {
            get
            {
                return this._preparedCommand;
            }
        }

        public ISqlMapper SqlMap
        {
            get
            {
                return this._sqlMap;
            }
        }

        public IStatement Statement
        {
            get
            {
                return this._statement;
            }
        }
    }
}

