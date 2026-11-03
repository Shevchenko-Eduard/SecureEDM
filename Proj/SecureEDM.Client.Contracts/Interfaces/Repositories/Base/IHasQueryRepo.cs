using System.Collections.ObjectModel;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories.Base;

public interface IHasQueryRepo<TDto, TResult>
{
    Task<ReadOnlySet<TResult>> QueryAsync(TDto dto, CancellationToken token = default);
}