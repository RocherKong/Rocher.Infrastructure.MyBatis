namespace IBatisNet.DataMapper.Configuration.Cache.Memory
{
    using IBatisNet.DataMapper.Configuration.Cache;
    using System;
    using System.Collections;
    using System.Reflection;

    public class MemoryCacheControler : ICacheController
    {
        private Hashtable _cache = Hashtable.Synchronized(new Hashtable());
        private MemoryCacheLevel _cacheLevel = MemoryCacheLevel.Weak;

        public void Configure(IDictionary properties)
        {
            string str = (string) properties["Type"];
            if (str != null)
            {
                this._cacheLevel = MemoryCacheLevel.GetByRefenceType(str.ToUpper());
            }
        }

        public void Flush()
        {
            lock (this)
            {
                this._cache.Clear();
            }
        }

        public object Remove(object key)
        {
            object target = null;
            object obj3 = this[key];
            this._cache.Remove(key);
            if (obj3 != null)
            {
                if (obj3 is StrongReference)
                {
                    return ((StrongReference) obj3).Target;
                }
                if (obj3 is WeakReference)
                {
                    target = ((WeakReference) obj3).Target;
                }
            }
            return target;
        }

        public object this[object key]
        {
            get
            {
                object target = null;
                object obj3 = this._cache[key];
                if (obj3 != null)
                {
                    if (obj3 is StrongReference)
                    {
                        return ((StrongReference) obj3).Target;
                    }
                    if (obj3 is WeakReference)
                    {
                        target = ((WeakReference) obj3).Target;
                    }
                }
                return target;
            }
            set
            {
                object obj2 = null;
                if (this._cacheLevel.Equals(MemoryCacheLevel.Weak))
                {
                    obj2 = new WeakReference(value);
                }
                else if (this._cacheLevel.Equals(MemoryCacheLevel.Strong))
                {
                    obj2 = new StrongReference(value);
                }
                this._cache[key] = obj2;
            }
        }

        private class StrongReference
        {
            private object _target;

            public StrongReference(object obj)
            {
                this._target = obj;
            }

            public object Target
            {
                get
                {
                    return this._target;
                }
            }
        }
    }
}

