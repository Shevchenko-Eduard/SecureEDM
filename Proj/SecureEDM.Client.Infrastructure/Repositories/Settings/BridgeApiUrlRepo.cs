using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class BridgeApiUrlRepo(ISettingProvider provider) : IBridgeApiUrlRepo
{
    private readonly ISettingProvider _provider = provider;

    public async ValueTask<Uri> Get(CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        return model.BridgeApiUrl;
    }

    public async ValueTask Update(Uri value, CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        model.BridgeApiUrl = value;
        await _provider.Update(model, token);
    }
}