namespace IBatisNet.DataMapper.Configuration.Statements
{
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Data;
    using System.Text;

    public class PreparedStatementFactory
    {
        private string _commandText = string.Empty;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string _parameterPrefix = string.Empty;
        private PreparedStatement _preparedStatement;
        private HybridDictionary _propertyDbParameterMap = new HybridDictionary();
        private RequestScope _request;
        private ISqlMapSession _session;
        private IStatement _statement;

        public PreparedStatementFactory(ISqlMapSession session, RequestScope request, IStatement statement, string commandText)
        {
            this._session = session;
            this._request = request;
            this._statement = statement;
            this._commandText = commandText;
        }

        private void CreateParametersForProcedureCommand()
        {
            string columnName = string.Empty;
            string parameterDbTypeProperty = this._session.DataSource.DbProvider.ParameterDbTypeProperty;
            Type parameterDbType = this._session.DataSource.DbProvider.ParameterDbType;
            ParameterPropertyCollection properties = null;
            if (this._session.DataSource.DbProvider.UsePositionalParameters)
            {
                properties = this._request.ParameterMap.Properties;
            }
            else
            {
                properties = this._request.ParameterMap.PropertiesList;
            }
            this._preparedStatement.DbParameters = new IDbDataParameter[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                ParameterProperty key = properties[i];
                if (this._session.DataSource.DbProvider.UseParameterPrefixInParameter)
                {
                    columnName = this._parameterPrefix + key.ColumnName;
                }
                else
                {
                    columnName = key.ColumnName;
                }
                IDbDataParameter parameter = this._session.CreateCommand(this._statement.CommandType).CreateParameter();
                if ((key.DbType != null) && (key.DbType.Length > 0))
                {
                    object memberValue = Enum.Parse(parameterDbType, key.DbType, true);
                    ObjectProbe.SetMemberValue(parameter, parameterDbTypeProperty, memberValue, this._request.DataExchangeFactory.ObjectFactory, this._request.DataExchangeFactory.AccessorFactory);
                }
                if (this._session.DataSource.DbProvider.SetDbParameterSize && (key.Size != -1))
                {
                    parameter.Size = key.Size;
                }
                if (this._session.DataSource.DbProvider.SetDbParameterPrecision)
                {
                    parameter.Precision = key.Precision;
                }
                if (this._session.DataSource.DbProvider.SetDbParameterScale)
                {
                    parameter.Scale = key.Scale;
                }
                parameter.Direction = key.Direction;
                parameter.ParameterName = columnName;
                this._preparedStatement.DbParametersName.Add(key.PropertyName);
                this._preparedStatement.DbParameters[i] = parameter;
                if (!this._session.DataSource.DbProvider.UsePositionalParameters)
                {
                    this._propertyDbParameterMap.Add(key, parameter);
                }
            }
        }

        private void CreateParametersForTextCommand()
        {
            string str = string.Empty;
            string parameterDbTypeProperty = this._session.DataSource.DbProvider.ParameterDbTypeProperty;
            Type parameterDbType = this._session.DataSource.DbProvider.ParameterDbType;
            ParameterPropertyCollection properties = null;
            if (this._session.DataSource.DbProvider.UsePositionalParameters)
            {
                properties = this._request.ParameterMap.Properties;
            }
            else
            {
                properties = this._request.ParameterMap.PropertiesList;
            }
            this._preparedStatement.DbParameters = new IDbDataParameter[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                ParameterProperty key = properties[i];
                if (this._session.DataSource.DbProvider.UseParameterPrefixInParameter)
                {
                    str = this._parameterPrefix + "param" + i;
                }
                else
                {
                    str = "param" + i;
                }
                IDbDataParameter parameter = this._session.CreateDataParameter();
                if ((key.DbType != null) && (key.DbType.Length > 0))
                {
                    object memberValue = Enum.Parse(parameterDbType, key.DbType, true);
                    ObjectProbe.SetMemberValue(parameter, parameterDbTypeProperty, memberValue, this._request.DataExchangeFactory.ObjectFactory, this._request.DataExchangeFactory.AccessorFactory);
                }
                if (this._session.DataSource.DbProvider.SetDbParameterSize && (key.Size != -1))
                {
                    parameter.Size = key.Size;
                }
                if (this._session.DataSource.DbProvider.SetDbParameterPrecision)
                {
                    parameter.Precision = key.Precision;
                }
                if (this._session.DataSource.DbProvider.SetDbParameterScale)
                {
                    parameter.Scale = key.Scale;
                }
                parameter.Direction = key.Direction;
                parameter.ParameterName = str;
                this._preparedStatement.DbParametersName.Add(key.PropertyName);
                this._preparedStatement.DbParameters[i] = parameter;
                if (!this._session.DataSource.DbProvider.UsePositionalParameters)
                {
                    this._propertyDbParameterMap.Add(key, parameter);
                }
            }
        }

        private void DiscoverParameter(ISqlMapSession session)
        {
            IDataParameter[] spParameterSet = session.SqlMapper.DBHelperParameterCache.GetSpParameterSet(session, this._commandText);
            this._preparedStatement.DbParameters = new IDbDataParameter[spParameterSet.Length];
            int length = session.DataSource.DbProvider.ParameterPrefix.Length;
            for (int i = 0; i < spParameterSet.Length; i++)
            {
                IDbDataParameter parameter = (IDbDataParameter) spParameterSet[i];
                if (!session.DataSource.DbProvider.UseParameterPrefixInParameter && parameter.ParameterName.StartsWith(session.DataSource.DbProvider.ParameterPrefix))
                {
                    parameter.ParameterName = parameter.ParameterName.Substring(length);
                }
                this._preparedStatement.DbParametersName.Add(parameter.ParameterName);
                this._preparedStatement.DbParameters[i] = parameter;
            }
            IDbDataParameter[] parameterArray2 = new IDbDataParameter[spParameterSet.Length];
            for (int j = 0; j < this._statement.ParameterMap.Properties.Count; j++)
            {
                parameterArray2[j] = this.Search(session, this._preparedStatement.DbParameters, this._statement.ParameterMap.Properties[j], j);
            }
            this._preparedStatement.DbParameters = parameterArray2;
        }

        private void EvaluateParameterMap()
        {
            string delimiters = "?";
            string current = null;
            int num = 0;
            string parameterName = string.Empty;
            StringTokenizer tokenizer = new StringTokenizer(this._commandText, delimiters, true);
            StringBuilder builder = new StringBuilder();
            IEnumerator enumerator = tokenizer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                current = (string) enumerator.Current;
                if (delimiters.Equals(current))
                {
                    ParameterProperty property = this._request.ParameterMap.Properties[num];
                    IDataParameter parameter = null;
                    if (this._session.DataSource.DbProvider.UsePositionalParameters)
                    {
                        if (this._parameterPrefix.Equals(":"))
                        {
                            parameterName = ":" + num;
                        }
                        else
                        {
                            parameterName = "?";
                        }
                    }
                    else
                    {
                        parameter = (IDataParameter) this._propertyDbParameterMap[property];
                        if (this._session.DataSource.DbProvider.UseParameterPrefixInParameter)
                        {
                            if (this._session.DataSource.DbProvider.Name.IndexOf("ByteFx") >= 0)
                            {
                                parameterName = this._parameterPrefix + parameter.ParameterName;
                            }
                            else
                            {
                                parameterName = parameter.ParameterName;
                            }
                        }
                        else
                        {
                            parameterName = this._parameterPrefix + parameter.ParameterName;
                        }
                    }
                    builder.Append(" ");
                    builder.Append(parameterName);
                    parameterName = string.Empty;
                    num++;
                }
                else
                {
                    builder.Append(current);
                }
            }
            this._preparedStatement.PreparedSql = builder.ToString();
        }

        public PreparedStatement Prepare()
        {
            this._preparedStatement = new PreparedStatement();
            this._parameterPrefix = this._session.DataSource.DbProvider.ParameterPrefix;
            this._preparedStatement.PreparedSql = this._commandText;
            if (this._statement.CommandType == CommandType.Text)
            {
                if (this._request.ParameterMap != null)
                {
                    this.CreateParametersForTextCommand();
                    this.EvaluateParameterMap();
                }
            }
            else if (this._statement.CommandType == CommandType.StoredProcedure)
            {
                if (this._request.ParameterMap == null)
                {
                    throw new DataMapperException("A procedure statement tag must have a parameterMap attribute, which is not the case for the procedure '" + this._statement.Id + ".");
                }
                if (this._session.DataSource.DbProvider.UseDeriveParameters)
                {
                    this.DiscoverParameter(this._session);
                }
                else
                {
                    this.CreateParametersForProcedureCommand();
                }
                if (this._session.DataSource.DbProvider.IsObdc)
                {
                    StringBuilder builder = new StringBuilder("{ call ");
                    builder.Append(this._commandText);
                    if (this._preparedStatement.DbParameters.Length > 0)
                    {
                        builder.Append(" (");
                        int num = this._preparedStatement.DbParameters.Length - 1;
                        for (int i = 0; i < num; i++)
                        {
                            builder.Append("?,");
                        }
                        builder.Append("?) }");
                    }
                    this._preparedStatement.PreparedSql = builder.ToString();
                }
            }
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Statement Id: [" + this._statement.Id + "] Prepared SQL: [" + this._preparedStatement.PreparedSql + "]");
            }
            return this._preparedStatement;
        }

        private IDbDataParameter Search(ISqlMapSession session, IDbDataParameter[] parameters, ParameterProperty property, int index)
        {
            if (property.ColumnName.Length <= 0)
            {
                return parameters[index];
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                string parameterName = parameters[i].ParameterName;
                if (session.DataSource.DbProvider.UseParameterPrefixInParameter && parameterName.StartsWith(session.DataSource.DbProvider.ParameterPrefix))
                {
                    int length = session.DataSource.DbProvider.ParameterPrefix.Length;
                    parameterName = parameterName.Substring(length);
                }
                if (property.ColumnName.Equals(parameterName))
                {
                    return parameters[i];
                }
            }
            throw new IndexOutOfRangeException("The parameter '" + property.ColumnName + "' does not exist in the stored procedure '" + this._statement.Id + "'. Check your parameterMap.");
        }
    }
}

