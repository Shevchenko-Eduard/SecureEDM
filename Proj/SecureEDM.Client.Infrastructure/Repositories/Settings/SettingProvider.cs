using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class SettingProvider : ISettingProvider
{
    private readonly ICache<bool, SettingModel> _cache;
    private readonly ISerializer<SettingModel> _serializer;

    public SettingProvider(ICache<bool, SettingModel> cache, ISerializer<SettingModel> serializer)
    {
        _serializer = serializer;
        _cache = cache;
        _cache.Init<SettingProvider>(timerInterval: new(0, 1, 0));
    }

    public async ValueTask<SettingModel> Get(CancellationToken token = default)
    {
        if (await _cache.Exists(true)) return await _cache.Get(true);

        using var fs = new FileStream(AppDataDir.Setting, FileMode.OpenOrCreate, FileAccess.Read);
        if (fs.Length == 0)
            return await _cache.GetOrSet(true, SettingDefaults.Create());

        try
        {
            var value = await _serializer.DeserializeAsync(fs, token);
            return await _cache.GetOrSet(true, value);
        }
        catch
        {
            return await _cache.GetOrSet(true, SettingDefaults.Create());
        }
    }

    public async ValueTask Update(SettingModel value, CancellationToken token = default)
    {
        using var fs = new FileStream(AppDataDir.Setting, FileMode.Create, FileAccess.Write);
        using var ss = await _serializer.SerializeAsync(value);
        await ss.CopyToAsync(fs);
        await _cache.Set(true, value);
    }
}