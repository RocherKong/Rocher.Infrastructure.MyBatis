namespace IBatisNet.DataMapper
{
    using IBatisNet.Common;
    using System;

    public interface ISqlMapSession : IDalSession, IDisposable
    {
        void CreateConnection();
        void CreateConnection(string connectionString);

        ISqlMapper SqlMapper { get; }
    }
}

