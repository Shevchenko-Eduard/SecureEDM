using SecureEDM.Client.Application.Models;

namespace SecureEDM.Client.Application.Interfaces;

public interface IRegistryStore
{
    ValueTask<EdoPersistedState?> LoadAsync(CancellationToken token = default);
    ValueTask SaveAsync(EdoPersistedState state, CancellationToken token = default);
}
