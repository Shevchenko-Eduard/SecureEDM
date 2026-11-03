namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasCreateRepo<TDto>
{
    Task CreateAsync(TDto dto, CancellationToken token = default);
}