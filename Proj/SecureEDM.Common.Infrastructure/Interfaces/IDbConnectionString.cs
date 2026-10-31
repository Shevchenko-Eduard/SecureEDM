namespace SecureEDM.Common.Infrastructure.Interfaces;

public interface IDbConnectionString
{
    public string GetConnectionString();
    public Task<string> GetConnectionStringAsync();
}