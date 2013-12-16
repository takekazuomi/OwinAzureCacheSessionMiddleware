using System;
using Microsoft.ApplicationServer.Caching;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Owin.Middleware
{
    public class AzureCacheClient
    {
        private static readonly Lazy<DataCacheFactory> Factory = new Lazy<DataCacheFactory>(() => new DataCacheFactory());
        private RetryPolicy<CacheTransientErrorDetectionStrategy> _retryPolicy;
        private readonly DataCache _dataCache;

        public Action<string> Logger { get; set; }

        internal AzureCacheClient(string name = null)
        {
            _dataCache = name == null ? Factory.Value.GetDefaultCache() : Factory.Value.GetCache(name);

            RetryStrategy = new FixedInterval(10, TimeSpan.FromMilliseconds(100));

        }

        public T GetOrAdd<T>(string key, Func<string, T> valueFactory)
        {
            var item = _retryPolicy.ExecuteAction(() => _dataCache.GetCacheItem(key));

            if (item != null)
                return (T) item.Value;
            try
            {
                var value = valueFactory(key);
                _retryPolicy.ExecuteAction(() => _dataCache.Add(key, value));
                return value;
            }
            catch (DataCacheException e)
            {
                if (e.ErrorCode == DataCacheErrorCode.KeyAlreadyExists)
                {
                    return GetOrAdd(key, valueFactory);
                }
                throw;
            }
        }

        internal bool TryAdd<T>(string key, T value)
        {
            try
            {
                _retryPolicy.ExecuteAction(() => _dataCache.Add(key, value));
                return true;
            }
            catch (DataCacheException e)
            {
                if (e.ErrorCode == DataCacheErrorCode.KeyAlreadyExists)
                    return false;
                throw;
            }
        }

        public long Increment(string key, long value, long initialValue)
        {
            return _retryPolicy.ExecuteAction(() => _dataCache.Increment(key, value, initialValue));
        }


        internal T TryGet<T>(string key)
        {
            var value = _retryPolicy.ExecuteAction(() => _dataCache.GetCacheItem(key));
            return (T) (value == null ? null : value.Value);

        }

        internal void Put<T>(string key, T value)
        {
            _retryPolicy.ExecuteAction(() => _dataCache.Put(key, value));
        }


        private RetryStrategy RetryStrategy
        {
            set { _retryPolicy = new RetryPolicy<CacheTransientErrorDetectionStrategy>(value); }
        }
    }
}

