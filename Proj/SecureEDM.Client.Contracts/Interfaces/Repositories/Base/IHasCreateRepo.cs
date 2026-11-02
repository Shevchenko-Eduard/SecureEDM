namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasCreateRepo<TEntiry>
{
    Task CreateAsync(TEntiry entiry, CancellationToken token = default);
}