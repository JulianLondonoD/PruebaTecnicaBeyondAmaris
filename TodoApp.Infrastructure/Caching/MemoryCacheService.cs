using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TodoApp.Infrastructure.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(60);

    public MemoryCacheService(
        IMemoryCache memoryCache,
        ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("Cache hit para la clave: {Key}", key);
                return Task.FromResult(value);
            }

            _logger.LogDebug("Cache miss para la clave: {Key}", key);
            return Task.FromResult<T?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener del cache la clave: {Key}", key);
            return Task.FromResult<T?>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cacheExpiration = expiration ?? _defaultExpiration;
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(cacheExpiration)
                .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpiration.TotalMinutes / 2));

            _memoryCache.Set(key, value, cacheEntryOptions);
            
            _logger.LogDebug(
                "Valor establecido en cache para la clave: {Key} con expiración: {Expiration}",
                key,
                cacheExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al establecer en cache la clave: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Clave eliminada del cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar del cache la clave: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia en cache de la clave: {Key}", key);
            return Task.FromResult(false);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        
        if (cachedValue != null)
        {
            return cachedValue;
        }

        _logger.LogDebug("Creando nuevo valor para la clave: {Key}", key);
        
        var value = await factory();
        
        await SetAsync(key, value, expiration, cancellationToken);
        
        return value;
    }
}
