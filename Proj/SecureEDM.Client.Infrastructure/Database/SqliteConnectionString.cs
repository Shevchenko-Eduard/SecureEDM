using Microsoft.Data.Sqlite;
using SecureEDM.Common.Infrastructure.Interfaces;

namespace SecureEDM.Client.Infrastructure.Database;

public class SqliteConnectionString : IDbConnectionString
{
    public string GetConnectionString()
    {
        SqliteConnectionStringBuilder builder = new()
        {
            Cache = SqliteCacheMode.Shared,
            DataSource = SqlitePath.GetPath(),
            Mode = SqliteOpenMode.ReadWriteCreate,
            DefaultTimeout = 30
        };
        return builder.ConnectionString;
    }

    public async Task<string> GetConnectionStringAsync() => GetConnectionString();
}