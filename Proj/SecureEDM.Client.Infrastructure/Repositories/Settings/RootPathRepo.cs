using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class RootPathRepo(ISettingProvider provider) : IRootPathRepo
{
    private readonly ISettingProvider _provider = provider;

    public async ValueTask<string> Get(CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        return model.RootPath;
    }

    public async ValueTask Update(string value, CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        model.RootPath = value;
        await _provider.Update(model, token);
    }
}