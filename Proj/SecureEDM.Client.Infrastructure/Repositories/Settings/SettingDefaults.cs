using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Infrastructure.Repositories.Settings;

public static class SettingDefaults
{
    public static SettingModel Create() => new(
        authApiUrl: new Uri("https://localhost:5001/"),
        bridgeApiUrl: new Uri("https://localhost:5002/"),
        defaulteConflictStrategy: EnumConflictStrategy.KeepLocal,
        rootPath: Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SecureEDM"),
        syncInterval: TimeSpan.FromMinutes(5));
}
