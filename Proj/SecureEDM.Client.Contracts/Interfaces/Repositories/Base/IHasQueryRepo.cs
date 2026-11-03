namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasQueryRepo<TDto, TResult>
{
    Task<TResult> QueryAsync(TDto dto, CancellationToken token = default);
}