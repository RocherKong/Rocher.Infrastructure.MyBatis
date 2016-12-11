namespace IBatisNet.DataMapper.Configuration.ParameterMapping
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Utilities;
    using IBatisNet.Common.Utilities.Objects;
    using IBatisNet.DataMapper.Configuration.Sql.Dynamic;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Exceptions;
    using IBatisNet.DataMapper.Scope;
    using IBatisNet.DataMapper.TypeHandlers;
    using System;
    using System.Collections;
    using System.Text;

    internal class InlineParameterMapParser
    {
        private const string PARAM_DELIM = ":";
        private const string PARAMETER_TOKEN = "#";

        private ParameterProperty NewParseMapping(string token, Type parameterClassType, IScope scope)
        {
            ParameterProperty property = new ParameterProperty();
            IEnumerator enumerator = new StringTokenizer(token, "=,", false).GetEnumerator();
            enumerator.MoveNext();
            property.PropertyName = ((string) enumerator.Current).Trim();
            while (enumerator.MoveNext())
            {
                string current = (string) enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    throw new DataMapperException("Incorrect inline parameter map format (missmatched name=value pairs): " + token);
                }
                string str2 = (string) enumerator.Current;
                if (!"type".Equals(current))
                {
                    if (!"dbType".Equals(current))
                    {
                        if (!"direction".Equals(current))
                        {
                            if (!"nullValue".Equals(current))
                            {
                                if (!"handler".Equals(current))
                                {
                                    throw new DataMapperException("Unrecognized parameter mapping field: '" + current + "' in " + token);
                                }
                                property.CallBackName = str2;
                            }
                            else
                            {
                                property.NullValue = str2;
                            }
                        }
                        else
                        {
                            property.DirectionAttribute = str2;
                        }
                        continue;
                    }
                    property.DbType = str2;
                }
                else
                {
                    property.CLRType = str2;
                    continue;
                }
            }
            if (property.CallBackName.Length > 0)
            {
                property.Initialize(scope, parameterClassType);
                return property;
            }
            ITypeHandler unkownTypeHandler = null;
            if (parameterClassType == null)
            {
                unkownTypeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
            }
            else
            {
                unkownTypeHandler = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, property.PropertyName, property.CLRType, property.DbType);
            }
            property.TypeHandler = unkownTypeHandler;
            property.Initialize(scope, parameterClassType);
            return property;
        }

        private ParameterProperty OldParseMapping(string token, Type parameterClassType, IScope scope)
        {
            ParameterProperty property = new ParameterProperty();
            if (token.IndexOf(":") > -1)
            {
                StringTokenizer tokenizer = new StringTokenizer(token, ":", true);
                IEnumerator enumerator = tokenizer.GetEnumerator();
                int tokenNumber = tokenizer.TokenNumber;
                if (tokenNumber == 3)
                {
                    enumerator.MoveNext();
                    string str = ((string) enumerator.Current).Trim();
                    property.PropertyName = str;
                    enumerator.MoveNext();
                    enumerator.MoveNext();
                    string str2 = ((string) enumerator.Current).Trim();
                    property.DbType = str2;
                    ITypeHandler unkownTypeHandler = null;
                    if (parameterClassType == null)
                    {
                        unkownTypeHandler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                    }
                    else
                    {
                        unkownTypeHandler = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, str, null, str2);
                    }
                    property.TypeHandler = unkownTypeHandler;
                    property.Initialize(scope, parameterClassType);
                    return property;
                }
                if (tokenNumber < 5)
                {
                    throw new ConfigurationException("Incorrect inline parameter map format: " + token);
                }
                enumerator.MoveNext();
                string propertyName = ((string) enumerator.Current).Trim();
                enumerator.MoveNext();
                enumerator.MoveNext();
                string dbType = ((string) enumerator.Current).Trim();
                enumerator.MoveNext();
                enumerator.MoveNext();
                string str5 = ((string) enumerator.Current).Trim();
                while (enumerator.MoveNext())
                {
                    str5 = str5 + ((string) enumerator.Current).Trim();
                }
                property.PropertyName = propertyName;
                property.DbType = dbType;
                property.NullValue = str5;
                ITypeHandler handler2 = null;
                if (parameterClassType == null)
                {
                    handler2 = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
                else
                {
                    handler2 = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, propertyName, null, dbType);
                }
                property.TypeHandler = handler2;
                property.Initialize(scope, parameterClassType);
                return property;
            }
            property.PropertyName = token;
            ITypeHandler handler3 = null;
            if (parameterClassType == null)
            {
                handler3 = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
            }
            else
            {
                handler3 = this.ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory, parameterClassType, token, null, null);
            }
            property.TypeHandler = handler3;
            property.Initialize(scope, parameterClassType);
            return property;
        }

        public SqlText ParseInlineParameterMap(IScope scope, IStatement statement, string sqlStatement)
        {
            string str = sqlStatement;
            ArrayList list = new ArrayList();
            Type parameterClassType = null;
            if (statement != null)
            {
                parameterClassType = statement.ParameterClass;
            }
            StringTokenizer tokenizer = new StringTokenizer(sqlStatement, "#", true);
            StringBuilder builder = new StringBuilder();
            string current = null;
            string str3 = null;
            IEnumerator enumerator = tokenizer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                current = (string) enumerator.Current;
                if ("#".Equals(str3))
                {
                    if ("#".Equals(current))
                    {
                        builder.Append("#");
                        current = null;
                    }
                    else
                    {
                        ParameterProperty property = null;
                        if (current.IndexOf(":") > -1)
                        {
                            property = this.OldParseMapping(current, parameterClassType, scope);
                        }
                        else
                        {
                            property = this.NewParseMapping(current, parameterClassType, scope);
                        }
                        list.Add(property);
                        builder.Append("? ");
                        enumerator.MoveNext();
                        current = (string) enumerator.Current;
                        if (!"#".Equals(current))
                        {
                            throw new DataMapperException("Unterminated inline parameter in mapped statement (" + statement.Id + ").");
                        }
                        current = null;
                    }
                }
                else if (!"#".Equals(current))
                {
                    builder.Append(current);
                }
                str3 = current;
            }
            str = builder.ToString();
            ParameterProperty[] propertyArray = (ParameterProperty[]) list.ToArray(typeof(ParameterProperty));
            return new SqlText { Text = str, Parameters = propertyArray };
        }

        private ITypeHandler ResolveTypeHandler(TypeHandlerFactory typeHandlerFactory, Type parameterClassType, string propertyName, string propertyType, string dbType)
        {
            if (parameterClassType == null)
            {
                return typeHandlerFactory.GetUnkownTypeHandler();
            }
            if (typeof(IDictionary).IsAssignableFrom(parameterClassType))
            {
                if ((propertyType == null) || (propertyType.Length == 0))
                {
                    return typeHandlerFactory.GetUnkownTypeHandler();
                }
                try
                {
                    Type type = TypeUtils.ResolveType(propertyType);
                    return typeHandlerFactory.GetTypeHandler(type, dbType);
                }
                catch (Exception exception)
                {
                    throw new ConfigurationException("Error. Could not set TypeHandler.  Cause: " + exception.Message, exception);
                }
            }
            if (typeHandlerFactory.GetTypeHandler(parameterClassType, dbType) != null)
            {
                return typeHandlerFactory.GetTypeHandler(parameterClassType, dbType);
            }
            Type memberTypeForGetter = ObjectProbe.GetMemberTypeForGetter(parameterClassType, propertyName);
            return typeHandlerFactory.GetTypeHandler(memberTypeForGetter, dbType);
        }
    }
}

