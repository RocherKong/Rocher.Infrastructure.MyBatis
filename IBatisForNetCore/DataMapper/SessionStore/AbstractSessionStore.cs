namespace IBatisNet.DataMapper.SessionStore
{
    using IBatisNet.DataMapper;
    using System;

    public abstract class AbstractSessionStore : MarshalByRefObject, ISessionStore
    {
        private const string KEY = "_IBATIS_LOCAL_SQLMAP_SESSION_";
        protected string sessionName = string.Empty;

        public AbstractSessionStore(string sqlMapperId)
        {
            this.sessionName = "_IBATIS_LOCAL_SQLMAP_SESSION_" + sqlMapperId;
        }

        public abstract void Dispose();
        public abstract void Store(ISqlMapSession session);

        public abstract ISqlMapSession LocalSession { get; }
    }
}

