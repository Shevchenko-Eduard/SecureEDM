using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class RootPathRepo : IRootPathRepo
{
    public ValueTask<string> Get(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask Update(string value, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}