namespace SecureEDM.Common.Contracts.Interfaces;

public interface ICache<TKey, TValue> : IDisposable
    where TKey : notnull
    where TValue : notnull
{
    ValueTask Init<TObject>(
        string? backetName = null,
        TimeSpan timerInterval = default
    );
    ValueTask InitGlobal(
        string backetName,
        TimeSpan timerInterval = default
    );
    ValueTask Set(TKey key, TValue value);
    ValueTask Delete(TKey key);
    ValueTask<bool> Exists(TKey key);
    ValueTask<TValue> Get(TKey key);
    ValueTask<TValue> GetOrSet(TKey key, TValue value);
    ValueTask Flush();
}