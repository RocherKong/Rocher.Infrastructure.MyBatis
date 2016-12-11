namespace IBatisNet.DataMapper.Configuration.Sql.Dynamic
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.Sql;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Elements;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic.Handlers;
    using IBatisNet.DataMapper.Configuration.Sql.SimpleDynamic;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.MappedStatements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Text;

    internal sealed class DynamicSql : ISql, IDynamicParent
    {
        private IList _children = new ArrayList();
        private DataExchangeFactory _dataExchangeFactory;
        private InlineParameterMapParser _paramParser;
        private IStatement _statement;
        private bool _usePositionalParameters;

        internal DynamicSql(ConfigurationScope configScope, IStatement statement)
        {
            this._statement = statement;
            this._usePositionalParameters = configScope.DataSource.DbProvider.UsePositionalParameters;
            this._dataExchangeFactory = configScope.DataExchangeFactory;
        }

        public void AddChild(ISqlChild child)
        {
            this._children.Add(child);
        }

        private PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string sqlStatement)
        {
            PreparedStatementFactory factory = new PreparedStatementFactory(session, request, this._statement, sqlStatement);
            return factory.Prepare();
        }

        public RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session)
        {
            RequestScope request = new RequestScope(this._dataExchangeFactory, session, this._statement);
            this._paramParser = new InlineParameterMapParser();
            string sqlStatement = this.Process(request, parameterObject);
            request.PreparedStatement = this.BuildPreparedStatement(session, request, sqlStatement);
            request.MappedStatement = mappedStatement;
            return request;
        }

        private string Process(RequestScope request, object parameterObject)
        {
            SqlTagContext ctx = new SqlTagContext();
            IList localChildren = this._children;
            this.ProcessBodyChildren(request, ctx, parameterObject, localChildren);
            ParameterMap map = new ParameterMap(request.DataExchangeFactory) {
                Id = this._statement.Id + "-InlineParameterMap"
            };
            map.Initialize(this._usePositionalParameters, request);
            map.Class = this._statement.ParameterClass;
            IList parameterMappings = ctx.GetParameterMappings();
            int count = parameterMappings.Count;
            for (int i = 0; i < count; i++)
            {
                map.AddParameterProperty((ParameterProperty) parameterMappings[i]);
            }
            request.ParameterMap = map;
            string bodyText = ctx.BodyText;
            if (SimpleDynamicSql.IsSimpleDynamicSql(bodyText))
            {
                bodyText = new SimpleDynamicSql(request, bodyText, this._statement).GetSql(parameterObject);
            }
            return bodyText;
        }

        private void ProcessBodyChildren(RequestScope request, SqlTagContext ctx, object parameterObject, IList localChildren)
        {
            StringBuilder writer = ctx.GetWriter();
            this.ProcessBodyChildren(request, ctx, parameterObject, localChildren.GetEnumerator(), writer);
        }

        private void ProcessBodyChildren(RequestScope request, SqlTagContext ctx, object parameterObject, IEnumerator localChildren, StringBuilder buffer)
        {
            while (localChildren.MoveNext())
            {
                ISqlChild current = (ISqlChild) localChildren.Current;
                if (current is SqlText)
                {
                    SqlText text = (SqlText) current;
                    string str = text.Text;
                    if (text.IsWhiteSpace)
                    {
                        buffer.Append(str);
                    }
                    else
                    {
                        buffer.Append(" ");
                        buffer.Append(str);
                        ParameterProperty[] parameters = text.Parameters;
                        if (parameters != null)
                        {
                            int length = parameters.Length;
                            for (int i = 0; i < length; i++)
                            {
                                ctx.AddParameterMapping(parameters[i]);
                            }
                        }
                    }
                }
                else if (current is SqlTag)
                {
                    SqlTag tag = (SqlTag) current;
                    ISqlTagHandler handler = tag.Handler;
                    int num3 = 1;
                    do
                    {
                        StringBuilder builder = new StringBuilder();
                        num3 = handler.DoStartFragment(ctx, tag, parameterObject);
                        if (num3 != 0)
                        {
                            if ((ctx.IsOverridePrepend && (ctx.FirstNonDynamicTagWithPrepend == null)) && (tag.IsPrependAvailable && !(tag.Handler is DynamicTagHandler)))
                            {
                                ctx.FirstNonDynamicTagWithPrepend = tag;
                            }
                            this.ProcessBodyChildren(request, ctx, parameterObject, tag.GetChildrenEnumerator(), builder);
                            num3 = handler.DoEndFragment(ctx, tag, parameterObject, builder);
                            handler.DoPrepend(ctx, tag, parameterObject, builder);
                            if ((num3 != 0) && (builder.Length > 0))
                            {
                                if (handler.IsPostParseRequired)
                                {
                                    SqlText text2 = this._paramParser.ParseInlineParameterMap(request, null, builder.ToString());
                                    buffer.Append(text2.Text);
                                    ParameterProperty[] propertyArray2 = text2.Parameters;
                                    if (propertyArray2 != null)
                                    {
                                        int num4 = propertyArray2.Length;
                                        for (int j = 0; j < num4; j++)
                                        {
                                            ctx.AddParameterMapping(propertyArray2[j]);
                                        }
                                    }
                                }
                                else
                                {
                                    buffer.Append(" ");
                                    buffer.Append(builder.ToString());
                                }
                                if (tag.IsPrependAvailable && (tag == ctx.FirstNonDynamicTagWithPrepend))
                                {
                                    ctx.IsOverridePrepend = false;
                                }
                            }
                        }
                    }
                    while (num3 == 2);
                }
            }
        }
    }
}

