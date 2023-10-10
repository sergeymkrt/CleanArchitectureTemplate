using System.Text.Json;
using CleanArchitectureTemplate.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArchitectureTemplate.Infrastructure.Shared.CacheManager;

public class CacheProvider : ICacheProvider
{
    private readonly IDistributedCache _cache;

    public CacheProvider(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetFromCache<T>(string key)
    {
        var cachedResponse = await _cache.GetStringAsync(key);
        return cachedResponse == null ? default(T) : JsonSerializer.Deserialize<T>(cachedResponse);
    }

    public async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options)
    {
        var response = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, response, options);
    }

    public async Task ClearCache(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<bool> HasKey(string key)
    {
        var cachedResponse = await _cache.GetStringAsync(key);
        return cachedResponse != null;
    }
}
