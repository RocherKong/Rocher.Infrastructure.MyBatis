namespace IBatisNet.DataMapper.SessionStore
{
    using IBatisNet.DataMapper;
    using System;
    using System.Runtime.Remoting.Messaging;
    using System.Web;

    public class HybridWebThreadSessionStore : AbstractSessionStore
    {
        public HybridWebThreadSessionStore(string sqlMapperId) : base(sqlMapperId)
        {
        }

        public override void Dispose()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                CallContext.SetData(base.sessionName, null);
            }
            else
            {
                current.Items.Remove(base.sessionName);
            }
        }

        public override void Store(ISqlMapSession session)
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                CallContext.SetData(base.sessionName, session);
            }
            else
            {
                current.Items[base.sessionName] = session;
            }
        }

        public override ISqlMapSession LocalSession
        {
            get
            {
                HttpContext current = HttpContext.Current;
                if (current == null)
                {
                    return (CallContext.GetData(base.sessionName) as SqlMapSession);
                }
                return (current.Items[base.sessionName] as SqlMapSession);
            }
        }
    }
}

