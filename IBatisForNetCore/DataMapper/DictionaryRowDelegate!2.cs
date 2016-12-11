namespace IBatisNet.DataMapper
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public delegate void DictionaryRowDelegate<K, V>(K key, V value, object parameterObject, IDictionary<K, V> dictionary);
}

