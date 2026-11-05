using SecureEDM.Client.Contracts.Interfaces;
using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class Setting(
    IAuthApiUrlRepo authApiUrl,
    IBridgeApiUrlRepo bridgeApiUrl,
    IDefaulteConflictStrategyRepo defaulteConflictStrategy,
    IRootPathRepo rootPath,
    ISyncIntervalRepo syncInterval) : ISetting
{
    public IAuthApiUrlRepo AuthApiUrl => authApiUrl;

    public IBridgeApiUrlRepo BridgeApiUrl => bridgeApiUrl;

    public IDefaulteConflictStrategyRepo DefaulteConflictStrategy => defaulteConflictStrategy;

    public IRootPathRepo RootPath => rootPath;

    public ISyncIntervalRepo SyncInterval => syncInterval;
}