namespace IBatisNet.DataMapper.SessionStore
{
    using IBatisNet.DataMapper;
    using System;
    using System.Runtime.Remoting.Messaging;

    public class CallContextSessionStore : AbstractSessionStore
    {
        public CallContextSessionStore(string sqlMapperId) : base(sqlMapperId)
        {
        }

        public override void Dispose()
        {
            CallContext.SetData(base.sessionName, null);
        }

        public override void Store(ISqlMapSession session)
        {
            CallContext.SetData(base.sessionName, session);
        }

        public override ISqlMapSession LocalSession
        {
            get
            {
                return (CallContext.GetData(base.sessionName) as SqlMapSession);
            }
        }
    }
}

