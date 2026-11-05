using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Contracts.Interfaces;
using SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;
using SecureEDM.Client.Infrastructure.Database;

namespace SecureEDM.Client.Infrastructure;

public class UnitOfWorkClient(
    IAsymmetricKeyRepo asymmetricKey,
    IHashOfDocumentRepo hashOfDocument,
    IPrivateKeyRepo privateKey,
    IPublicKeyRepo publicKey,
    ISignatureRepo signature,
    ITrackingDocumentRepo trackingDocument,
    ClientContext context
) : IUnitOfWorkClient
{
    public IAsymmetricKeyRepo AsymmetricKey => asymmetricKey;

    public IHashOfDocumentRepo HashOfDocument => hashOfDocument;

    public IPrivateKeyRepo PrivateKey => privateKey;

    public IPublicKeyRepo PublicKey => publicKey;

    public ISignatureRepo Signature => signature;

    public ITrackingDocumentRepo TrackingDocument => trackingDocument;

    private readonly ClientContext _context = context;

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken token = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(token);

                try
                {
                    await action(token);
                    await _context.SaveChangesAsync(token);
                    await transaction.CommitAsync(token);
                }
                catch
                {
                    await transaction.RollbackAsync(token);
                    throw;
                }
            }
        );
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken token = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(token);

                try
                {
                    var result = await action(token);
                    await _context.SaveChangesAsync(token);
                    await transaction.CommitAsync(token);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(token);
                    throw;
                }
            }
        );
    }

    public async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return await _context.SaveChangesAsync(token);
    }
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}