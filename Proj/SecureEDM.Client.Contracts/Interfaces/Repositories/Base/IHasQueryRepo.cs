namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasQueryRepo<TQuery>
{
    Task QueryAsync(TQuery query, CancellationToken token = default);
}