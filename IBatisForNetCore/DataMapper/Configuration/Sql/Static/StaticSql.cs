namespace IBatisNet.DataMapper.Configuration.Sql.Static
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Sql;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;

    public sealed class StaticSql : ISql
    {
        private DataExchangeFactory _dataExchangeFactory;
        private PreparedStatement _preparedStatement;
        private IStatement _statement;

        public StaticSql(IScope scope, IStatement statement)
        {
            this._statement = statement;
            this._dataExchangeFactory = scope.DataExchangeFactory;
        }

        public void BuildPreparedStatement(ISqlMapSession session, string sqlStatement)
        {
            RequestScope request = new RequestScope(this._dataExchangeFactory, session, this._statement);
            this._preparedStatement = new PreparedStatementFactory(session, request, this._statement, sqlStatement).Prepare();
        }

        public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
        {
            return new RequestScope(this._dataExchangeFactory, session, this._statement) { PreparedStatement = this._preparedStatement, MappedStatement = mappedStatement };
        }
    }
}

