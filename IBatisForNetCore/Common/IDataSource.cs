namespace IBatisNet.Common
{
    using System;

    public interface IDataSource
    {
        string ConnectionString { get; set; }

        IDbProvider DbProvider { get; set; }

        string Name { get; set; }
    }
}

