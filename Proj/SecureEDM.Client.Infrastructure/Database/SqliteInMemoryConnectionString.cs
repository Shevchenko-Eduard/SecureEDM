using Microsoft.Data.Sqlite;
using SecureEDM.Common.Infrastructure.Interfaces;

namespace SecureEDM.Client.Infrastructure.Database;

public class SqliteInMemoryConnectionString : IDbConnectionString
{
    public string GetConnectionString()
    {
        SqliteConnectionStringBuilder builder = new()
        {
            Mode = SqliteOpenMode.Memory
        };
        return builder.ConnectionString;
    }

    public async Task<string> GetConnectionStringAsync() => GetConnectionString();
}