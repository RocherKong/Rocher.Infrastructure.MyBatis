namespace IBatisNet.DataMapper.Scope
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.DataExchange;
    using IBatisNet.DataMapper.MappedStatements;
    using System;
    using System.Collections;
    using System.Data;
    using System.Runtime.CompilerServices;

    public class RequestScope : IScope
    {
        private System.Data.IDbCommand _command;
        private int _currentResultMapIndex = -1;
        private IBatisNet.DataMapper.DataExchange.DataExchangeFactory _dataExchangeFactory;
        private IBatisNet.DataMapper.Scope.ErrorContext _errorContext = new IBatisNet.DataMapper.Scope.ErrorContext();
        private long _id;
        private IMappedStatement _mappedStatement;
        private static long _nextId;
        private IBatisNet.DataMapper.Configuration.ParameterMapping.ParameterMap _parameterMap;
        private IBatisNet.DataMapper.Configuration.Statements.PreparedStatement _preparedStatement;
        private bool _rowDataFound;
        private Queue _selects = new Queue();
        private ISqlMapSession _session;
        private IStatement _statement;
        private IDictionary _uniqueKeys;

        public RequestScope(IBatisNet.DataMapper.DataExchange.DataExchangeFactory dataExchangeFactory, ISqlMapSession session, IStatement statement)
        {
            this._statement = statement;
            this._parameterMap = statement.ParameterMap;
            this._session = session;
            this._dataExchangeFactory = dataExchangeFactory;
            this._id = GetNextId();
        }

        public override bool Equals(object obj)
        {
            if (this != obj)
            {
                if (!(obj is RequestScope))
                {
                    return false;
                }
                RequestScope scope = (RequestScope) obj;
                if (this._id != scope._id)
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return (int) (this._id ^ (this._id >> 0x20));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static long GetNextId()
        {
            _nextId += 1L;
            return _nextId;
        }

        public IDictionary GetUniqueKeys(IResultMap map)
        {
            if (this._uniqueKeys == null)
            {
                return null;
            }
            return (IDictionary) this._uniqueKeys[map];
        }

        public bool MoveNextResultMap()
        {
            if (this._currentResultMapIndex < (this._statement.ResultsMap.Count - 1))
            {
                this._currentResultMapIndex++;
                return true;
            }
            return false;
        }

        public void SetUniqueKeys(IResultMap map, IDictionary keys)
        {
            if (this._uniqueKeys == null)
            {
                this._uniqueKeys = new Hashtable();
            }
            this._uniqueKeys.Add(map, keys);
        }

        public IResultMap CurrentResultMap
        {
            get
            {
                return this._statement.ResultsMap[this._currentResultMapIndex];
            }
        }

        public IBatisNet.DataMapper.DataExchange.DataExchangeFactory DataExchangeFactory
        {
            get
            {
                return this._dataExchangeFactory;
            }
        }

        public IBatisNet.DataMapper.Scope.ErrorContext ErrorContext
        {
            get
            {
                return this._errorContext;
            }
        }

        public System.Data.IDbCommand IDbCommand
        {
            get
            {
                return this._command;
            }
            set
            {
                this._command = value;
            }
        }

        public bool IsRowDataFound
        {
            get
            {
                return this._rowDataFound;
            }
            set
            {
                this._rowDataFound = value;
            }
        }

        public IMappedStatement MappedStatement
        {
            get
            {
                return this._mappedStatement;
            }
            set
            {
                this._mappedStatement = value;
            }
        }

        public IBatisNet.DataMapper.Configuration.ParameterMapping.ParameterMap ParameterMap
        {
            get
            {
                return this._parameterMap;
            }
            set
            {
                this._parameterMap = value;
            }
        }

        public IBatisNet.DataMapper.Configuration.Statements.PreparedStatement PreparedStatement
        {
            get
            {
                return this._preparedStatement;
            }
            set
            {
                this._preparedStatement = value;
            }
        }

        public Queue QueueSelect
        {
            get
            {
                return this._selects;
            }
            set
            {
                this._selects = value;
            }
        }

        public ISqlMapSession Session
        {
            get
            {
                return this._session;
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

