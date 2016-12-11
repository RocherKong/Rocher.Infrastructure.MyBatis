namespace IBatisNet.DataMapper.Proxy
{
    using Castle.DynamicProxy;
    using IBatisNet.Common.Logging;
    using IBatisNet.Common.Utilities.Objects.Members;
    using IBatisNet.DataMapper;
    using IBatisNet.DataMapper.MappedStatements;
    using System;
    using System.Collections;

    [Serializable]
    internal class LazyLoadInterceptor : IInterceptor
    {
        private object _lazyLoadedItem;
        private bool _loaded;
        private object _loadLock = new object();
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private object _param;
        private static ArrayList _passthroughMethods = new ArrayList();
        private ISetAccessor _setAccessor;
        private ISqlMapper _sqlMap;
        private string _statementName = string.Empty;
        private object _target;

        static LazyLoadInterceptor()
        {
            _passthroughMethods.Add("GetType");
        }

        internal LazyLoadInterceptor(IMappedStatement mappedSatement, object param, object target, ISetAccessor setAccessor)
        {
            this._param = param;
            this._statementName = mappedSatement.Id;
            this._sqlMap = mappedSatement.SqlMap;
            this._target = target;
            this._setAccessor = setAccessor;
        }

        public object Intercept(IInvocation invocation, params object[] arguments)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Proxyfying call to " + invocation.Method.Name);
            }
            lock (this._loadLock)
            {
                if (!this._loaded && !_passthroughMethods.Contains(invocation.Method.Name))
                {
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.Debug("Proxyfying call, query statement " + this._statementName);
                    }
                    if (typeof(IList).IsAssignableFrom(this._setAccessor.MemberType))
                    {
                        this._lazyLoadedItem = this._sqlMap.QueryForList(this._statementName, this._param);
                    }
                    else
                    {
                        this._lazyLoadedItem = this._sqlMap.QueryForObject(this._statementName, this._param);
                    }
                    this._loaded = true;
                    this._setAccessor.Set(this._target, this._lazyLoadedItem);
                }
            }
            object obj2 = invocation.Method.Invoke(this._lazyLoadedItem, arguments);
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("End of proxyfied call to " + invocation.Method.Name);
            }
            return obj2;
        }
    }
}

