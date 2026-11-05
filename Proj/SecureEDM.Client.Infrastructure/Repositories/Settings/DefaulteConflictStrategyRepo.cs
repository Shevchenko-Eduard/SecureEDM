using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;
using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class DefaulteConflictStrategyRepo : IDefaulteConflictStrategyRepo
{
    public ValueTask<EnumConflictStrategy> Get(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask Update(EnumConflictStrategy value, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}