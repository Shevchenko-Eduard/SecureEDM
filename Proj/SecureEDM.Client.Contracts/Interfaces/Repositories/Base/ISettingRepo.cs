namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface ISettingRepo<TValue>
{
    ValueTask<TValue> Get(CancellationToken token = default);
    ValueTask Update(TValue value, CancellationToken token = default);
}