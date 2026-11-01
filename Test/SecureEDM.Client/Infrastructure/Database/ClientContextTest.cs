using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SecureEDM.Common.Domain.Models.ValueObject;
using SecureEDM.Common.Infrastructure;

namespace SecureEDM.Client.Infrastructure.Database;

public class ClientContextTest
{
    public ClientContextTest()
    {
        GlobalClock.SetClock(new Clock());
    }
    private async Task<(DbContextOptions<ClientContext> Options, SqliteConnection Connection)> CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection(new SqliteInMemoryConnectionString().GetConnectionString());
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ClientContext>()
            .UseSqlite(connection)
            .Options;

        return (options, connection);
    }
    private async Task<bool> IsSuccessfully(Task task)
    {
        await task;
        return task.IsCompletedSuccessfully;
    }
    [Fact]
    public async Task CreatedAndDeletedDatabase()
    {
        (var options, var connection) = await CreateInMemoryDatabase();
        try
        {
            using ClientContext db = new(options);
            Assert.True(await db.Database.EnsureCreatedAsync());
            Console.WriteLine(db.Database.GenerateCreateScript());
            Assert.True(await db.Database.EnsureDeletedAsync());
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
    [Fact]
    public async Task OpenAndCloseConnectionDatabase()
    {
        (var options, var connection) = await CreateInMemoryDatabase();
        try
        {
            using ClientContext db = new(options);
            Assert.True(await db.Database.EnsureCreatedAsync());
            Assert.True(await IsSuccessfully(db.Database.OpenConnectionAsync()));
            Assert.True(await IsSuccessfully(db.Database.CloseConnectionAsync()));
            Assert.True(await db.Database.EnsureDeletedAsync());
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
    [Fact]
    public async Task BeginAndCommitTransactionDatabase()
    {
        (var options, var connection) = await CreateInMemoryDatabase();
        try
        {
            using ClientContext db = new(options);
            Assert.True(await db.Database.EnsureCreatedAsync());
            var transaction = db.Database.BeginTransactionAsync();
            Assert.True(await IsSuccessfully(transaction));
            Assert.True(await IsSuccessfully((await transaction).CommitAsync()));
            Assert.True(await db.Database.EnsureDeletedAsync());
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
}