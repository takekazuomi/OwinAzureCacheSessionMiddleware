using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Owin.Middleware
{
    public class SessionMiddleware : OwinMiddleware
    {
        public const string SessionKeyName = "Kyrt.Session";

        public SessionMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            string sessionId = null;
            try
            {
                sessionId = AzureCacheSessionProvidor.PreInvoke(context, SessionKeyName);
            }
            catch (Exception e)
            {
                context.TraceOutput.WriteLine(e);
            }

            return Next.Invoke(context).ContinueWith((task, state) =>
            {
                try
                {
                    var p = state as Tuple<IOwinContext, string>; 
                    if (p!=null && p.Item2 != null)
                        AzureCacheSessionProvidor.PostInvoke( p.Item1, SessionKeyName, p.Item2);
                }
                catch (Exception e)
                {
                    context.TraceOutput.WriteLine(e);
                }
                return task;
            }, Tuple.Create(context, sessionId));
        }
    }
}


