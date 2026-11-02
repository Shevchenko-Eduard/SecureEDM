namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasDeleteRepo<TId>
{
    Task DeleteAsync(TId id, CancellationToken token = default);
}