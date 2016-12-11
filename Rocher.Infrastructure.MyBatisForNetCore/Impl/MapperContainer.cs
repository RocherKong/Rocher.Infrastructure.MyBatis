using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocher.Infrastructure.MyBatis
{
    public sealed class MapperContainer
    {
        private static Hashtable _mapperContainer = new Hashtable();
        private static readonly object syncRoot = new object();

        public static void Configure(object obj)
        {
            _mapperContainer.Clear();
        }

        public static ISqlMapper GetInstance(string SqlMapConfigPath = "SqlMap.config")
        {
            if (_mapperContainer[SqlMapConfigPath] == null)
            {
                object syncRoot = MapperContainer.syncRoot;
                lock (syncRoot)
                {
                    if (_mapperContainer[SqlMapConfigPath] == null)
                    {
                        ISqlMapper mapper = new DomSqlMapBuilder().ConfigureAndWatch(SqlMapConfigPath, new ConfigureHandler(MapperContainer.Configure));
                        _mapperContainer.Add(SqlMapConfigPath, mapper);
                    }
                }
            }
            return (_mapperContainer[SqlMapConfigPath] as ISqlMapper);
        }
    }
}
