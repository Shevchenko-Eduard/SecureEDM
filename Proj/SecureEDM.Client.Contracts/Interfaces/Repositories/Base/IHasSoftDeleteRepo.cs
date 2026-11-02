namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasSoftDeleteRepo<TEntiry>
{
    Task SoftDeleteAsync(TEntiry entiry, CancellationToken token = default);
}