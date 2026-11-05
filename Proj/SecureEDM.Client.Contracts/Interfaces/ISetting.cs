using SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

namespace SecureEDM.Client.Contracts.Interfaces;

public interface ISetting
{
    IAuthApiUrlRepo AuthApiUrl { get; }
    IBridgeApiUrlRepo BridgeApiUrl { get; }
    IDefaulteConflictStrategyRepo DefaulteConflictStrategy { get; }
    IRootPathRepo RootPath { get; }
    ISyncIntervalRepo SyncInterval { get; }
}