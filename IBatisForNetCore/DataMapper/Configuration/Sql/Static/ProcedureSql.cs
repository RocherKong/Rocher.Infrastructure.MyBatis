namespace IBatisNet.DataMapper.Configuration.Sql.Static
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Sql;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;

    public sealed class ProcedureSql : ISql
    {
        private DataExchangeFactory _dataExchangeFactory;
        private PreparedStatement _preparedStatement;
        private string _sqlStatement = string.Empty;
        private IStatement _statement;
        private object _synRoot = new object();

        public ProcedureSql(IScope scope, string sqlStatement, IStatement statement)
        {
            this._sqlStatement = sqlStatement;
            this._statement = statement;
            this._dataExchangeFactory = scope.DataExchangeFactory;
        }

        public PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string commandText)
        {
            if (this._preparedStatement == null)
            {
                lock (this._synRoot)
                {
                    if (this._preparedStatement == null)
                    {
                        this._preparedStatement = new PreparedStatementFactory(session, request, this._statement, commandText).Prepare();
                    }
                }
            }
            return this._preparedStatement;
        }

        public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
        {
            RequestScope scope;
            return new RequestScope(this._dataExchangeFactory, session, this._statement) { PreparedStatement = this.BuildPreparedStatement(session, scope, this._sqlStatement), MappedStatement = mappedStatement };
        }
    }
}

