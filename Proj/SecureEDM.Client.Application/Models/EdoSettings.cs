using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Application.Models;

/// <summary>Локальные настройки клиента.</summary>
public sealed class EdoSettings
{
    public Uri AuthApiUrl { get; set; } = new("https://localhost:7001");
    public Uri BridgeApiUrl { get; set; } = new("https://localhost:7002");
    public EnumConflictStrategy DefaulteConflictStrategy { get; set; } = EnumConflictStrategy.KeepLocal;
    public string RootPath { get; set; } = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SecureEDM");
    public TimeSpan SyncInterval { get; set; } = TimeSpan.FromMinutes(5);
}
