using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;
using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class DefaulteConflictStrategyRepo(ISettingProvider provider) : IDefaulteConflictStrategyRepo
{
    private readonly ISettingProvider _provider = provider;

    public async ValueTask<EnumConflictStrategy> Get(CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        return model.DefaulteConflictStrategy;
    }

    public async ValueTask Update(EnumConflictStrategy value, CancellationToken token = default)
    {
        var model = await _provider.Get(token);
        model.DefaulteConflictStrategy = value;
        await _provider.Update(model, token);
    }
}