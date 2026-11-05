using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class AuthApiUrlRepo(ISettingProvider provider) : IAuthApiUrlRepo
{
    private readonly ISettingProvider _provider = provider;
    public async ValueTask<Uri> Get(CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        return model.AuthApiUrl;
    }

    public async ValueTask Update(Uri value, CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        model.AuthApiUrl = value;
        await _provider.Update(model, token);
    }
}