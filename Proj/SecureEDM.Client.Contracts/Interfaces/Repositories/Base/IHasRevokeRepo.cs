namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasRevokeRepo<TEntiry>
{
    Task RevokeAsync(TEntiry entiry, CancellationToken token = default);
}