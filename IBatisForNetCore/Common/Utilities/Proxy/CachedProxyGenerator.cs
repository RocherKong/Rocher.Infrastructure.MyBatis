namespace IBatisNet.Common.Utilities.Proxy
{
    using Castle.DynamicProxy;
    using IBatisNet.Common.Exceptions;
    using IBatisNet.Common.Logging;
    using System;
    using System.Collections;

    [CLSCompliant(false)]
    public class CachedProxyGenerator : ProxyGenerator
    {
        private IDictionary _cachedProxyTypes = new HybridDictionary();
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override object CreateClassProxy(Type targetType, IInterceptor interceptor, params object[] argumentsForConstructor)
        {
            object obj2;
            try
            {
                Type type = null;
                lock (this._cachedProxyTypes.SyncRoot)
                {
                    type = this._cachedProxyTypes[targetType] as Type;
                    if (type == null)
                    {
                        type = base.ProxyBuilder.CreateClassProxy(targetType);
                        this._cachedProxyTypes[targetType] = type;
                    }
                }
                obj2 = this.CreateClassProxyInstance(type, interceptor, argumentsForConstructor);
            }
            catch (Exception exception)
            {
                _log.Error("Castle Dynamic Class-Proxy Generator failed", exception);
                throw new IBatisNetException("Castle Proxy Generator failed", exception);
            }
            return obj2;
        }

        public override object CreateProxy(Type theInterface, IInterceptor interceptor, object target)
        {
            return this.CreateProxy(new Type[] { theInterface }, interceptor, target);
        }

        public override object CreateProxy(Type[] interfaces, IInterceptor interceptor, object target)
        {
            object obj2;
            try
            {
                Type type = null;
                Type type2 = target.GetType();
                lock (this._cachedProxyTypes.SyncRoot)
                {
                    type = this._cachedProxyTypes[type2] as Type;
                    if (type == null)
                    {
                        type = base.ProxyBuilder.CreateInterfaceProxy(interfaces, type2);
                        this._cachedProxyTypes[type2] = type;
                    }
                }
                obj2 = base.CreateProxyInstance(type, interceptor, target);
            }
            catch (Exception exception)
            {
                _log.Error("Castle Dynamic Proxy Generator failed", exception);
                throw new IBatisNetException("Castle Proxy Generator failed", exception);
            }
            return obj2;
        }
    }
}

