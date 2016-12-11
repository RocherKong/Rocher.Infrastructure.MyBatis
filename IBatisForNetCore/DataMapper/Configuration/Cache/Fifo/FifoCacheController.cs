namespace IBatisNet.DataMapper.Configuration.Cache.Fifo
{
    using IBatisNet.DataMapper.Configuration.Cache;
    using System;
    using System.Collections;
    using System.Reflection;

    public class FifoCacheController : ICacheController
    {
        private Hashtable _cache = Hashtable.Synchronized(new Hashtable());
        private int _cacheSize = 100;
        private IList _keyList = ArrayList.Synchronized(new ArrayList());

        public void Configure(IDictionary properties)
        {
            string str = (string) properties["CacheSize"];
            if (str != null)
            {
                this._cacheSize = Convert.ToInt32(str);
            }
        }

        public void Flush()
        {
            this._cache.Clear();
            this._keyList.Clear();
        }

        public object Remove(object key)
        {
            object obj2 = this[key];
            this._keyList.Remove(key);
            this._cache.Remove(key);
            return obj2;
        }

        public object this[object key]
        {
            get
            {
                return this._cache[key];
            }
            set
            {
                this._cache[key] = value;
                this._keyList.Add(key);
                if (this._keyList.Count > this._cacheSize)
                {
                    object obj2 = this._keyList[0];
                    this._keyList.Remove(0);
                    this._cache.Remove(obj2);
                }
            }
        }
    }
}

