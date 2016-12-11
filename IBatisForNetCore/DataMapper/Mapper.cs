namespace IBatisNet.DataMapper
{
    using IBatisNet.Common.Utilities;
    using IBatisNet.DataMapper.Configuration;
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Mapper
    {
        private static volatile ISqlMapper _mapper;

        public static void Configure(object obj)
        {
            _mapper = null;
        }

        public static ISqlMapper Get()
        {
            return Instance();
        }

        public static void InitMapper()
        {
            ConfigureHandler configureDelegate = new ConfigureHandler(Mapper.Configure);
            _mapper = new DomSqlMapBuilder().ConfigureAndWatch(configureDelegate);
        }

        public static ISqlMapper Instance()
        {
            if (_mapper == null)
            {
                lock (typeof(SqlMapper))
                {
                    if (_mapper == null)
                    {
                        InitMapper();
                    }
                }
            }
            return _mapper;
        }
    }
}

