using System.Security.Cryptography;
using SecureEDM.Client.Infrastructure.Database;

namespace SecureEDM.Client.Infrastructure.Database;

public static class SqlitePath
{
    private const string FileName = "SecureEDM.db";
    public static string GetPath()
    {
        string appDataDir = AppDataDir.GetPath();
        string path = Path.Combine(appDataDir, FileName);
        return path;
    }
}