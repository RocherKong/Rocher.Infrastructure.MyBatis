namespace IBatisNet.DataMapper.Configuration.Cache
{
    using System;
    using System.Collections;
    using System.Reflection;

    public interface ICacheController
    {
        void Configure(IDictionary properties);
        void Flush();
        object Remove(object key);

        object this[object key] { get; set; }
    }
}

