namespace IBatisNet.DataMapper.Configuration.Sql.SimpleDynamic
{
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.Sql;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Text;

    internal sealed class SimpleDynamicSql : ISql
    {
        private DataExchangeFactory _dataExchangeFactory;
        private string _simpleSqlStatement = string.Empty;
        private IStatement _statement;
        private const string ELEMENT_TOKEN = "$";

        internal SimpleDynamicSql(IScope scope, string sqlStatement, IStatement statement)
        {
            this._simpleSqlStatement = sqlStatement;
            this._statement = statement;
            this._dataExchangeFactory = scope.DataExchangeFactory;
        }

        private PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string sqlStatement)
        {
            PreparedStatementFactory factory = new PreparedStatementFactory(session, request, this._statement, sqlStatement);
            return factory.Prepare();
        }

        public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
        {
            RequestScope scope;
            string sqlStatement = this.ProcessDynamicElements(parameterObject);
            return new RequestScope(this._dataExchangeFactory, session, this._statement) { PreparedStatement = this.BuildPreparedStatement(session, scope, sqlStatement), MappedStatement = mappedStatement };
        }

        public string GetSql(object parameterObject)
        {
            return this.ProcessDynamicElements(parameterObject);
        }

        public static bool IsSimpleDynamicSql(string sqlStatement)
        {
            return ((sqlStatement != null) && (sqlStatement.IndexOf("$") > -1));
        }

        private string ProcessDynamicElements(object parameterObject)
        {
            StringTokenizer tokenizer = new StringTokenizer(this._simpleSqlStatement, "$", true);
            StringBuilder builder = new StringBuilder();
            string current = null;
            string str2 = null;
            IEnumerator enumerator = tokenizer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                current = (string) enumerator.Current;
                if ("$".Equals(str2))
                {
                    if ("$".Equals(current))
                    {
                        builder.Append("$");
                        current = null;
                    }
                    else
                    {
                        object obj2 = null;
                        if (parameterObject != null)
                        {
                            if (this._dataExchangeFactory.TypeHandlerFactory.IsSimpleType(parameterObject.GetType()))
                            {
                                obj2 = parameterObject;
                            }
                            else
                            {
                                obj2 = ObjectProbe.GetMemberValue(parameterObject, current, this._dataExchangeFactory.AccessorFactory);
                            }
                        }
                        if (obj2 != null)
                        {
                            builder.Append(obj2.ToString());
                        }
                        enumerator.MoveNext();
                        current = (string) enumerator.Current;
                        if (!"$".Equals(current))
                        {
                            throw new DataMapperException("Unterminated dynamic element in sql (" + this._simpleSqlStatement + ").");
                        }
                        current = null;
                    }
                }
                else if (!"$".Equals(current))
                {
                    builder.Append(current);
                }
                str2 = current;
            }
            return builder.ToString();
        }
    }
}

