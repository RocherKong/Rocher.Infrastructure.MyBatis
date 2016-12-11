namespace IBatisNet.Common.Utilities.Proxy
{
    using Castle.DynamicProxy;
    using IBatisNet.Common.Logging;
    using System;

    [CLSCompliant(false)]
    public sealed class ProxyGeneratorFactory
    {
        private static ProxyGenerator _generator = new CachedProxyGenerator();
        private static readonly ILog log = LogManager.GetLogger(typeof(ProxyGeneratorFactory));

        private ProxyGeneratorFactory()
        {
        }

        public static ProxyGenerator GetProxyGenerator()
        {
            return _generator;
        }
    }
}

