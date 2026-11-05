using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;
using SecureEDM.Common.Domain.Interfaces;

namespace SecureEDM.Client.Infrastructure.Database;

public class UnitOfWorkClientTest
{
    private (SqliteConnection connection, DbContextOptions<ClientContext> options) Init()
    {
        var connection = new SqliteConnection(new SqliteInMemoryConnectionString().GetConnectionString());
        var options = new DbContextOptionsBuilder<ClientContext>().UseSqlite(connection).Options;
        return (connection, options);
    }
    private UnitOfWorkClient InitUoW(ClientContext context)
    {
        return new(
            asymmetricKey: new Mock<IAsymmetricKeyRepo>().Object,
            hashOfDocument: new Mock<IHashOfDocumentRepo>().Object,
            publicKey: new Mock<IPublicKeyRepo>().Object,
            privateKey: new Mock<IPrivateKeyRepo>().Object,
            signature: new Mock<ISignatureRepo>().Object,
            trackingDocument: new Mock<ITrackingDocumentRepo>().Object,
            context: context
        );
    }
    [Fact]
    public async Task CreatedTransaction()
    {
        var (connection, options) = Init();
        using ClientContext context = new(options);
        await context.Database.EnsureCreatedAsync();
        var uow = InitUoW(context);
        var mockClock = new Mock<IClock>();
        mockClock.Setup(m => m.Now).Returns(new DateTimeOffset(new(2026, 4, 20)));
        try
        {
            await uow.ExecuteInTransactionAsync(async (token) =>
            {
                await context.SaveChangesAsync(token);
            });
        }
        finally
        {
            await context.Database.EnsureDeletedAsync();
            await connection.DisposeAsync();
        }
    }
}