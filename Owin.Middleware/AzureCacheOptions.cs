
namespace Owin.Middleware
{
    public class AzureCacheOptions
    {
        public AzureCacheOptions()
        {
            CacheName = null;
        }
        public string CacheName { get; set; }
    }
}
