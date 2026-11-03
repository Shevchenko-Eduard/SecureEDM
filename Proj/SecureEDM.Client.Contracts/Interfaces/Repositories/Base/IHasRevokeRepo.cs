namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasRevokeRepo<TDto>
{
    Task RevokeAsync(TDto dto, CancellationToken token = default);
}