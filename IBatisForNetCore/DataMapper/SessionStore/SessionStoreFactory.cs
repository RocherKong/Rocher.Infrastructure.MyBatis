namespace IBatisNet.DataMapper.SessionStore
{
    using System;
    using System.Web;

    public sealed class SessionStoreFactory
    {
        public static ISessionStore GetSessionStore(string sqlMapperId)
        {
            if (HttpContext.Current == null)
            {
                return new CallContextSessionStore(sqlMapperId);
            }
            return new WebSessionStore(sqlMapperId);
        }
    }
}

