using System;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;
using Microsoft.Owin;

namespace Owin.Middleware
{
    public class AzureCacheMiddleware : OwinMiddleware
    {
        public const string CacheKeyName = "Kyrt.CacheKeyName";

        private readonly AzureCacheOptions _options;

        public AzureCacheMiddleware(OwinMiddleware next, AzureCacheOptions options) : base(next)
        {
            _options = options ?? new AzureCacheOptions();
        }

        public override Task Invoke(IOwinContext context)
        {
            try
            {
                object cache;
                if (!context.Environment.TryGetValue(CacheKeyName, out cache))
                {
                    cache = new AzureCacheClient(_options.CacheName);
                    context.Environment[CacheKeyName] = cache;
                }
            }
            catch (DataCacheException e)
            {
                context.TraceOutput.WriteLine(e);
            }
            catch (Exception ex)
            {
                context.TraceOutput.WriteLine(ex);
                throw;
            }

            return Next.Invoke(context);
        }
    }
}

