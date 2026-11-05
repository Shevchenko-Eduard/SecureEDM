namespace SecureEDM.Client.Infrastructure;

public class CacheTimer<TKey>(TimeSpan timerInterval, Action<object> action) : IDisposable
    where TKey : notnull
{
    private Dictionary<TKey, Timer> Timers { get; } = [];
    private Dictionary<TKey, bool> IsExpired { get; } = [];
    private readonly TimeSpan TimerInterval = timerInterval;
    private readonly Action<object> Action = action;
    private int Milliseconds => (int)TimerInterval.TotalMilliseconds;
    private readonly Lock _lock = new();
    private void DeleteOldTimers()
    {
        lock (_lock)
        {
            TKey[] keys = [.. IsExpired
                .Where(e => e.Value == true)
                .Select(e => e.Key)];

            foreach (var key in keys)
            {
                DeleteTimers(key);
            }
        }
    }
    public void DeleteTimers(TKey key)
    {
        lock (_lock)
        {
            Timers[key].Dispose();
            Timers.Remove(key);
            IsExpired.Remove(key);
        }
    }
    public void SetTimer(TKey key, object value)
    {
        lock (_lock)
        {
            if (!Timers.ContainsKey(key))
            {
                Timers.Add(key, GetTimer(key, value));
                IsExpired.Add(key, false);
                return;
            }
            Timers[key].Change(Milliseconds, Timeout.Infinite);
            IsExpired[key] = false;
        }
    }
    private TimerCallback GetCallback(Action<object> action)
    {
        return new TimerCallback(async (value) =>
        {
            if (value is (TKey key, object externValue))
            {
                action(externValue);
                lock (_lock)
                {
                    DeleteOldTimers();
                    IsExpired[key] = true;
                }
            }
            else throw new ArgumentException("Not valid value.");
        });
    }
    private Timer GetTimer(TKey key, object value)
    {
        (TKey key, object externValue) innerValue = (key, value);
        return new(GetCallback(Action), innerValue, Milliseconds, Timeout.Infinite);
    }

    public void Dispose()
    {
        foreach (var t in Timers) t.Value.Dispose();
        Timers.Clear();
        IsExpired.Clear();
    }
}