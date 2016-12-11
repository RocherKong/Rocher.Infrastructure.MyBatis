namespace IBatisNet.DataMapper.SessionStore
{
    using IBatisNet.DataMapper;
    using System;

    public interface ISessionStore
    {
        void Dispose();
        void Store(ISqlMapSession session);

        ISqlMapSession LocalSession { get; }
    }
}

