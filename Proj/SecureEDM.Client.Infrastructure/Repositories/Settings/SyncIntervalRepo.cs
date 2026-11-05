using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class SyncIntervalRepo : ISyncIntervalRepo
{
    public ValueTask<TimeSpan> Get(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask Update(TimeSpan value, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}