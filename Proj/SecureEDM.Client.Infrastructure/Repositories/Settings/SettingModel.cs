using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public class SettingModel(
    Uri authApiUrl, Uri bridgeApiUrl,
    EnumConflictStrategy defaulteConflictStrategy,
    string rootPath, TimeSpan syncInterval
)
{
    public Uri AuthApiUrl { get; set; } = authApiUrl;
    public Uri BridgeApiUrl { get; set; } = bridgeApiUrl;
    public EnumConflictStrategy DefaulteConflictStrategy { get; set; } = defaulteConflictStrategy;
    public string RootPath { get; set; } = rootPath;
    public TimeSpan SyncInterval { get; set; } = syncInterval;
}