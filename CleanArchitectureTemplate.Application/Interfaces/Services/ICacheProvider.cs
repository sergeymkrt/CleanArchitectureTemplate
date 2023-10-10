using Microsoft.Extensions.Caching.Distributed;

namespace CleanArchitectureTemplate.Application.Interfaces.Services;

public interface ICacheProvider
{
    Task<T> GetFromCache<T>(string key);
    Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options);
    Task ClearCache(string key);
    Task<bool> HasKey(string key);
}
