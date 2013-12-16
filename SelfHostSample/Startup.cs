using System;
using System.Collections.Generic;
using Microsoft.Owin;
using Owin;
using Owin.Middleware;

[assembly: OwinStartup(typeof(SelfHostSample.Startup))]

namespace SelfHostSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseAzureCache();
            app.UseSession();

            app.Run(async context =>
            {
                context.TraceOutput.WriteLine("start app.Run {0}", context.Request.Path);

                context.Response.ContentType = "text/html";
                try
                {
                    var time = context.Cache().GetOrAdd("first time", s => DateTimeOffset.Now);
                    var count = context.Cache().Increment("counter", 1, 0);
                    int sessionCount = context.Session().Get("sessionCount", -1);
                    sessionCount++;
                    context.Session()["sessionCount"] = sessionCount;

                    //                    await context.TraceOutput.WriteLineAsync(context.DumpEnvironment(System.Environment.NewLine));

                    var msg = string.Format("Hello, World! {0} {1}/{2} {3}<br>", time.ToString(), sessionCount, count, context.Request.Path);
                    await context.Response.WriteAsync(msg);

                    //await context.Response.WriteAsync(context.DumpEnvironment("<br/>"));

                }
                catch (Exception e)
                {
                    context.TraceOutput.WriteLine(e);
                }
            });
            // app.UseWelcomePage();
        }
    }

    internal static class Extentions
    {
        public static T Get<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            object item;
            if (dictionary.TryGetValue(key, out item))
            {
                return (T) item;
            }
            return defaultValue;
        }
    }
}
