using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Application.Interfaces;

public interface ISetting
{
    public string RootPath { get; }
    public TimeSpan SyncInterval { get; }
    public EnumConflictStrategy DefaulteConflictStrategy { get; }
    public Uri BridgeApiBaseUrl { get; }
    public Uri AuthApiBaseUrl { get; }
    public Task SetRootPath(string path, CancellationToken token = default);
    public Task SetSyncInterval(TimeSpan interval, CancellationToken token = default);
    public Task SetDefaulteConflictStrategy(EnumConflictStrategy strategy, CancellationToken token = default);
    public Task SetBridgeApiBaseUrl(Uri uri, CancellationToken token = default);
    public Task SetAuthApiBaseUrl(Uri uri, CancellationToken token = default);
}