using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class SyncIntervalRepo(ISettingProvider provider) : ISyncIntervalRepo
{
    private readonly ISettingProvider _provider = provider;

    public async ValueTask<TimeSpan> Get(CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        return model.SyncInterval;
    }

    public async ValueTask Update(TimeSpan value, CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        model.SyncInterval = value;
        await _provider.Update(model, token);
    }
}