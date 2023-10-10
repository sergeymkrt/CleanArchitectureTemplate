using CleanArchitectureTemplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArchitectureTemplate.Infrastructure.Shared.CacheManager;

public class CacheService : ICacheService
{
    private readonly ICacheProvider _cacheProvider;

    public CacheService(ICacheProvider cacheProvider, IHttpContextAccessor httpContextAccessor)
    {
        _cacheProvider = cacheProvider;
    }

    public async Task<bool> HasKey(string key)
    {
        return await _cacheProvider.HasKey(key);
    }

    public async Task<T> GetSession<T>(string key)
    {
        return await _cacheProvider.GetFromCache<T>(key);
    }

    public async Task SetSession<T>(string key, T value)
    {
        var cacheEntryOptions = new DistributedCacheEntryOptions();
        await _cacheProvider.SetCache(key, value, cacheEntryOptions);
    }
}
