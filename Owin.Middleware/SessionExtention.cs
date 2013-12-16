using System;
using System.Collections.Concurrent;
using Microsoft.Owin;

namespace Owin.Middleware
{
    public static class SessionExtention
    {
        public static IAppBuilder UseSession(this IAppBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            return builder.Use(typeof(SessionMiddleware));
        }

        public static ConcurrentDictionary<string, object> Session(this IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return context.Environment[SessionMiddleware.SessionKeyName] as ConcurrentDictionary<string, object>;
        }
    }
}
