namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasSoftDeleteRepo<TDto>
{
    Task SoftDeleteAsync(TDto dto, CancellationToken token = default);
}