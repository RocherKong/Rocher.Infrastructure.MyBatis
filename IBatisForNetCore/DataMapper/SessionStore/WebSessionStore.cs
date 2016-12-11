namespace IBatisNet.DataMapper.SessionStore
{
    using IBatisNet.Common.Exceptions;
    using IBatisNet.DataMapper;
    using System;
    using System.Web;

    public class WebSessionStore : AbstractSessionStore
    {
        public WebSessionStore(string sqlMapperId) : base(sqlMapperId)
        {
        }

        public override void Dispose()
        {
            ObtainSessionContext().Items.Remove(base.sessionName);
        }

        private static HttpContext ObtainSessionContext()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new IBatisNetException("WebSessionStore: Could not obtain reference to HttpContext");
            }
            return current;
        }

        public override void Store(ISqlMapSession session)
        {
            ObtainSessionContext().Items[base.sessionName] = session;
        }

        public override ISqlMapSession LocalSession
        {
            get
            {
                return (ObtainSessionContext().Items[base.sessionName] as SqlMapSession);
            }
        }
    }
}

