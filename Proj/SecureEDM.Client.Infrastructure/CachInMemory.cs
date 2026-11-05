using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Client.Infrastructure;

public class CachInMemory<TKey, TValue>(IPoolCache poolCache) : ICache<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private Dictionary<object, object> Cache { get; set; } = null!;
    private TimeSpan TimerInterval { get; set; }
    private readonly Lock _lock = new();
    private readonly IPoolCache _poolCache = poolCache;
    private CacheTimer<TKey>? CacheTimer { get; set; }

    private CacheTimer<TKey> GetCacheTimer()
    {
        CacheTimer<TKey> timer = new(TimerInterval, (value) =>
        {
            if (value is (CachInMemory<TKey, TValue> cache, TKey key))
            {
                if (cache.Exists(key).Result) _ = cache.Delete(key);
            }
            else throw new ArgumentException("Not valid value.");
        });
        return timer;
    }

    private void SetTimer(TKey key)
    {
        if (CacheTimer == null) return;
        (CachInMemory<TKey, TValue> cache, TKey key) value = (this, key);
        CacheTimer.SetTimer(key, value);
    }

    private void StopTimer(TKey key)
    {
        if (CacheTimer == null) return;
        CacheTimer.DeleteTimers(key);
    }

    public async ValueTask Delete(TKey key)
    {
        CheckInit();
        if (await Exists(key))
        {
            StopTimer(key);
            Cache.Remove(key);
        }
        throw new ArgumentException("Key arnt exist.");
    }

    public async ValueTask<bool> Exists(TKey key)
    {
        CheckInit();
        return Cache.ContainsKey(key);
    }

    public async ValueTask Flush()
    {
        CheckInit();
        Cache.Clear();
    }

    public async ValueTask<TValue> Get(TKey key)
    {
        CheckInit();
        SetTimer(key);
        if (!Cache.TryGetValue(key, out var value)) throw new ArgumentException("Value arnt exist.");
        Converter(value, out TValue result);
        return result;
    }

    public async ValueTask<TValue> GetOrSet(TKey key, TValue value)
    {
        CheckInit();
        SetTimer(key);
        if (!Cache.TryGetValue(key, out var search))
        {
            await Set(key, value);
            search = value;
        }
        Converter(search, out TValue result);
        return result;
    }

    public async ValueTask Init<TObject>(string backetName, TimeSpan timerInterval = default)
    {
        Cache = _poolCache.Init(typeof(TObject), backetName);
        TimerInterval = timerInterval;
        if (TimerInterval != default) CacheTimer = GetCacheTimer();
    }

    public async ValueTask InitGlobal(string backetName, TimeSpan timerInterval = default)
    {
        if (!_poolCache.TryGet(null, backetName, out var cache)) cache = _poolCache.Init(null, backetName);
        Cache = cache!;
        TimerInterval = timerInterval;
        if (TimerInterval != default) CacheTimer = GetCacheTimer();
    }

    public async ValueTask Set(TKey key, TValue value)
    {
        CheckInit();
        SetTimer(key);
        if (await Exists(key)) Cache[key] = value;
        else Cache.Add(key, value);
    }

    private void CheckInit()
    {
        if (Cache == null) throw new ArgumentNullException("Cach dont init.");
    }
    private static void Converter<T>(object value, out T result)
    {
        if (value is T res) result = res;
        else throw new ArgumentException($"Value [{value.GetType().FullName}] is not {typeof(T).FullName}");
    }

    public void Dispose()
    {
        Cache.Clear();
        _poolCache.Dispose();
        CacheTimer?.Dispose();
    }
}