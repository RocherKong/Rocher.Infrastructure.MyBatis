namespace IBatisNet.DataMapper.MappedStatements
{
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.Commands;
    using IBatisNet.DataMapper.Configuration.Cache;
    using IBatisNet.DataMapper.Configuration.Statements;
    using IBatisNet.DataMapper.Scope;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;

    public sealed class CachingStatement : IMappedStatement
    {
        private MappedStatement _mappedStatement;

        public event ExecuteEventHandler Execute;

        public CachingStatement(MappedStatement statement)
        {
            this._mappedStatement = statement;
        }

        public object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            return this._mappedStatement.ExecuteInsert(session, parameterObject);
        }

        public IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            IDictionary<K, V> dictionary = new Dictionary<K, V>();
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
            this._mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);
            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForMap");
            if (keyProperty != null)
            {
                cacheKey.Update(keyProperty);
            }
            if (valueProperty != null)
            {
                cacheKey.Update(valueProperty);
            }
            dictionary = this.Statement.CacheModel[cacheKey] as IDictionary<K, V>;
            if (dictionary == null)
            {
                dictionary = this._mappedStatement.RunQueryForDictionary<K, V>(request, session, parameterObject, keyProperty, valueProperty, null);
                this.Statement.CacheModel[cacheKey] = dictionary;
            }
            return dictionary;
        }

        public IDictionary<K, V> ExecuteQueryForDictionary<K, V>(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
        {
            return this._mappedStatement.ExecuteQueryForDictionary<K, V>(session, parameterObject, keyProperty, valueProperty, rowDelegate);
        }

        public IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForList<T>(session, parameterObject, -1, -1);
        }

        public IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForList(session, parameterObject, -1, -1);
        }

        public void ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, IList<T> resultObject)
        {
            this._mappedStatement.ExecuteQueryForList<T>(session, parameterObject, resultObject);
        }

        public void ExecuteQueryForList(ISqlMapSession session, object parameterObject, IList resultObject)
        {
            this._mappedStatement.ExecuteQueryForList(session, parameterObject, resultObject);
        }

        public IList<T> ExecuteQueryForList<T>(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            IList<T> list = null;
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
            this._mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);
            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForList");
            cacheKey.Update(skipResults);
            cacheKey.Update(maxResults);
            list = this.Statement.CacheModel[cacheKey] as IList<T>;
            if (list == null)
            {
                list = this._mappedStatement.RunQueryForList<T>(request, session, parameterObject, skipResults, maxResults);
                this.Statement.CacheModel[cacheKey] = list;
            }
            return list;
        }

        public IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            IList list = null;
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
            this._mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);
            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForList");
            cacheKey.Update(skipResults);
            cacheKey.Update(maxResults);
            list = this.Statement.CacheModel[cacheKey] as IList;
            if (list == null)
            {
                list = this._mappedStatement.RunQueryForList(request, session, parameterObject, skipResults, maxResults);
                this.Statement.CacheModel[cacheKey] = list;
            }
            return list;
        }

        public IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            IDictionary dictionary = new Hashtable();
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
            this._mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);
            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForMap");
            if (keyProperty != null)
            {
                cacheKey.Update(keyProperty);
            }
            if (valueProperty != null)
            {
                cacheKey.Update(valueProperty);
            }
            dictionary = this.Statement.CacheModel[cacheKey] as IDictionary;
            if (dictionary == null)
            {
                dictionary = this._mappedStatement.RunQueryForMap(request, session, parameterObject, keyProperty, valueProperty, null);
                this.Statement.CacheModel[cacheKey] = dictionary;
            }
            return dictionary;
        }

        public IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            return this._mappedStatement.ExecuteQueryForMapWithRowDelegate(session, parameterObject, keyProperty, valueProperty, rowDelegate);
        }

        public object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForObject(session, parameterObject, null);
        }

        public T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForObject<T>(session, parameterObject, default(T));
        }

        public object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
        {
            object obj2 = null;
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
            this._mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);
            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForObject");
            obj2 = this.Statement.CacheModel[cacheKey];
            if (obj2 == CacheModel.NULL_OBJECT)
            {
                return null;
            }
            if (obj2 == null)
            {
                obj2 = this._mappedStatement.RunQueryForObject(request, session, parameterObject, resultObject);
                this.Statement.CacheModel[cacheKey] = obj2;
            }
            return obj2;
        }

        public T ExecuteQueryForObject<T>(ISqlMapSession session, object parameterObject, T resultObject)
        {
            T local = default(T);
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);
            this._mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);
            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForObject");
            object obj2 = this.Statement.CacheModel[cacheKey];
            if (obj2 is T)
            {
                return (T) obj2;
            }
            if (obj2 == CacheModel.NULL_OBJECT)
            {
                return default(T);
            }
            local = this._mappedStatement.RunQueryForObject<T>(request, session, parameterObject, resultObject);
            this.Statement.CacheModel[cacheKey] = local;
            return local;
        }

        public IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
        {
            return this._mappedStatement.ExecuteQueryForRowDelegate(session, parameterObject, rowDelegate);
        }

        public IList<T> ExecuteQueryForRowDelegate<T>(ISqlMapSession session, object parameterObject, RowDelegate<T> rowDelegate)
        {
            return this._mappedStatement.ExecuteQueryForRowDelegate<T>(session, parameterObject, rowDelegate);
        }

        public int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            return this._mappedStatement.ExecuteUpdate(session, parameterObject);
        }

        private CacheKey GetCacheKey(RequestScope request)
        {
            CacheKey key = new CacheKey();
            int count = request.IDbCommand.Parameters.Count;
            for (int i = 0; i < count; i++)
            {
                IDataParameter parameter = (IDataParameter) request.IDbCommand.Parameters[i];
                if (parameter.Value != null)
                {
                    key.Update(parameter.Value);
                }
            }
            key.Update(this._mappedStatement.Id);
            key.Update(this._mappedStatement.SqlMap.DataSource.ConnectionString);
            key.Update(request.IDbCommand.CommandText);
            CacheModel cacheModel = this._mappedStatement.Statement.CacheModel;
            if (!cacheModel.IsReadOnly && !cacheModel.IsSerializable)
            {
                key.Update(request);
            }
            return key;
        }

        public double GetDataCacheHitRatio()
        {
            if (this._mappedStatement.Statement.CacheModel != null)
            {
                return this._mappedStatement.Statement.CacheModel.HitRatio;
            }
            return -1.0;
        }

        public string Id
        {
            get
            {
                return this._mappedStatement.Id;
            }
        }

        public IPreparedCommand PreparedCommand
        {
            get
            {
                return this._mappedStatement.PreparedCommand;
            }
        }

        public ISqlMapper SqlMap
        {
            get
            {
                return this._mappedStatement.SqlMap;
            }
        }

        public IStatement Statement
        {
            get
            {
                return this._mappedStatement.Statement;
            }
        }
    }
}

