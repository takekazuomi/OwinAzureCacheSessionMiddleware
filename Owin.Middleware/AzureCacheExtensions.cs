using System;
using Microsoft.Owin;
using Owin;

namespace Owin.Middleware
{
    public static class AzureCacheExtensions
    {
        public static IAppBuilder UseAzureCache(this IAppBuilder builder, AzureCacheOptions options = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            return builder.Use(typeof (AzureCacheMiddleware), options);
        }

        public static AzureCacheClient Cache(this IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return context.Environment[AzureCacheMiddleware.CacheKeyName] as AzureCacheClient;
        }

    }

}
