namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasDeleteRepo<TDto>
{
    Task DeleteAsync(TDto dto, CancellationToken token = default);
}