namespace IBatisNet.DataMapper.Commands
{
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.Text;

    internal class DefaultPreparedCommand : IPreparedCommand
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected virtual void ApplyParameterMap(ISqlMapSession session, IDbCommand command, RequestScope request, IStatement statement, object parameterObject)
        {
            StringCollection dbParametersName = request.PreparedStatement.DbParametersName;
            IDbDataParameter[] dbParameters = request.PreparedStatement.DbParameters;
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            int count = dbParametersName.Count;
            for (int i = 0; i < count; i++)
            {
                IDbDataParameter parameter = dbParameters[i];
                IDbDataParameter dataParameter = command.CreateParameter();
                ParameterProperty mapping = request.ParameterMap.GetProperty(i);
                if (_logger.IsDebugEnabled)
                {
                    builder.Append(parameter.ParameterName);
                    builder.Append("=[");
                    builder2.Append(parameter.ParameterName);
                    builder2.Append("=[");
                }
                if (command.CommandType == CommandType.StoredProcedure)
                {
                    if (request.ParameterMap == null)
                    {
                        throw new DataMapperException("A procedure statement tag must alway have a parameterMap attribute, which is not the case for the procedure '" + statement.Id + "'.");
                    }
                    if (mapping.DirectionAttribute.Length == 0)
                    {
                        mapping.Direction = parameter.Direction;
                    }
                    parameter.Direction = mapping.Direction;
                }
                if (_logger.IsDebugEnabled)
                {
                    builder.Append(mapping.PropertyName);
                    builder.Append(",");
                }
                request.ParameterMap.SetParameter(mapping, dataParameter, parameterObject);
                dataParameter.Direction = parameter.Direction;
                if (((request.ParameterMap != null) && (mapping.DbType != null)) && (mapping.DbType.Length > 0))
                {
                    string parameterDbTypeProperty = session.DataSource.DbProvider.ParameterDbTypeProperty;
                    object memberValue = ObjectProbe.GetMemberValue(parameter, parameterDbTypeProperty, request.DataExchangeFactory.AccessorFactory);
                    ObjectProbe.SetMemberValue(dataParameter, parameterDbTypeProperty, memberValue, request.DataExchangeFactory.ObjectFactory, request.DataExchangeFactory.AccessorFactory);
                }
                if (_logger.IsDebugEnabled)
                {
                    if (dataParameter.Value == DBNull.Value)
                    {
                        builder.Append("null");
                        builder.Append("], ");
                        builder2.Append("System.DBNull, null");
                        builder2.Append("], ");
                    }
                    else
                    {
                        builder.Append(dataParameter.Value.ToString());
                        builder.Append("], ");
                        builder2.Append(dataParameter.DbType.ToString());
                        builder2.Append(", ");
                        builder2.Append(dataParameter.Value.GetType().ToString());
                        builder2.Append("], ");
                    }
                }
                if (session.DataSource.DbProvider.SetDbParameterSize && (parameter.Size > 0))
                {
                    dataParameter.Size = parameter.Size;
                }
                if (session.DataSource.DbProvider.SetDbParameterPrecision)
                {
                    dataParameter.Precision = parameter.Precision;
                }
                if (session.DataSource.DbProvider.SetDbParameterScale)
                {
                    dataParameter.Scale = parameter.Scale;
                }
                dataParameter.ParameterName = parameter.ParameterName;
                command.Parameters.Add(dataParameter);
            }
            if (_logger.IsDebugEnabled && (dbParametersName.Count > 0))
            {
                _logger.Debug("Statement Id: [" + statement.Id + "] Parameters: [" + builder.ToString(0, builder.Length - 2) + "]");
                _logger.Debug("Statement Id: [" + statement.Id + "] Types: [" + builder2.ToString(0, builder2.Length - 2) + "]");
            }
        }

        public void Create(RequestScope request, ISqlMapSession session, IStatement statement, object parameterObject)
        {
            request.IDbCommand = new DbCommandDecorator(session.CreateCommand(statement.CommandType), request);
            request.IDbCommand.CommandText = request.PreparedStatement.PreparedSql;
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Statement Id: [" + statement.Id + "] PreparedStatement : [" + request.IDbCommand.CommandText + "]");
            }
            this.ApplyParameterMap(session, request.IDbCommand, request, statement, parameterObject);
        }
    }
}

