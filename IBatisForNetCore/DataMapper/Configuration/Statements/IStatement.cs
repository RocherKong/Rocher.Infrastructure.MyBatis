namespace IBatisNet.DataMapper.Configuration.Statements
{
    using IBatisNet.DataMapper.Configuration.Cache;
    using IBatisNet.DataMapper.Configuration.ParameterMapping;
    using IBatisNet.DataMapper.Configuration.ResultMapping;
    using IBatisNet.DataMapper.Configuration.Sql;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public interface IStatement
    {
        IList<T> CreateInstanceOfGenericListClass<T>();
        IList CreateInstanceOfListClass();

        bool AllowRemapping { get; set; }

        IBatisNet.DataMapper.Configuration.Cache.CacheModel CacheModel { get; set; }

        string CacheModelName { get; set; }

        System.Data.CommandType CommandType { get; }

        string ExtendStatement { get; set; }

        string Id { get; set; }

        Type ListClass { get; }

        Type ParameterClass { get; }

        IBatisNet.DataMapper.Configuration.ParameterMapping.ParameterMap ParameterMap { get; set; }

        Type ResultClass { get; }

        ResultMapCollection ResultsMap { get; }

        ISql Sql { get; set; }
    }
}

