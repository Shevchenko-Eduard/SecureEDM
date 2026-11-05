using System.Text;
using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Client.Infrastructure;

public class PoolCachInMemory : IPoolCache
{
    private readonly Dictionary<string, Dictionary<object, object>> _pool = [];
    public Dictionary<object, object> Init(Type? type, string? backetName)
    {
        string key = KeyCombain(type, backetName);
        if (_pool.ContainsKey(key)) throw new ArgumentException($"{key} already exist in pool.");
        Dictionary<object, object> cache = [];
        _pool.Add(key, cache);
        return cache;
    }
    public bool TryGet(Type? type, string? backetName, out Dictionary<object, object>? cache)
    {
        string key = KeyCombain(type, backetName);
        if (!_pool.TryGetValue(key, out cache)) return false;
        return true;
    }
    public Dictionary<object, object> Get(Type? type, string? backetName) => Get(KeyCombain(type, backetName));
    private Dictionary<object, object> Get(string key)
    {
        if (!_pool.TryGetValue(key, out var cache)) throw new ArgumentException($"{key} not exist in pool.");
        return cache;
    }
    public void Clear(Type? type, string? backetName)
    {
        string key = KeyCombain(type, backetName);
        var cache = Get(key);
        cache.Clear();
        _pool.Remove(key);
    }
    private string KeyCombain(Type? type, string? backetName)
    {   
        List<string> builder = [];
        if (type != null && !string.IsNullOrWhiteSpace(type.FullName)) builder.Add(type.FullName);
        if (!string.IsNullOrWhiteSpace(backetName)) builder.Add(backetName);
        if (builder.Count == 0) throw new ArgumentNullException($"{nameof(type)} and {nameof(backetName)} is null or write space.");
        return string.Join('-', builder);
    }

    public void Dispose()
    {
        foreach (var p in _pool)
        {
            p.Value.Clear();
        }
        _pool.Clear();
    }
}