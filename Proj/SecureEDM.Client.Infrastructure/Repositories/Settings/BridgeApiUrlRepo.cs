using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class BridgeApiUrlRepo : IBridgeApiUrlRepo
{
    public ValueTask<Uri> Get(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask Update(Uri value, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}