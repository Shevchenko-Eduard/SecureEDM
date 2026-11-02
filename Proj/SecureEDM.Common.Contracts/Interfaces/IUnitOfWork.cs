namespace SecureEDM.Common.Contracts.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public Task<int> SaveChangesAsync(CancellationToken token = default);
    public Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken token = default);
    public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken token = default);
}